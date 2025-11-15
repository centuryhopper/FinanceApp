using Microsoft.AspNetCore.Mvc;
using Server.Services;
using Newtonsoft.Json;
using Going.Plaid.Auth;
using Server.Repositories;
using Shared.Models;
using System.Security.Claims;
using static Shared.Models.ServiceResponses;
using Microsoft.AspNetCore.Authorization;
using Server.Utils;
using System.Text;
using System.Text.Json;
using Going.Plaid.Transactions;
using Server.Contexts;
using LanguageExt;
using static LanguageExt.Prelude;
using Microsoft.AspNetCore.RateLimiting;

namespace Server.Controllers;


[ApiController]
[Route("api/[controller]")]
public class PlaidController : ControllerBase
{
    private readonly PlaidService plaidService;
    private readonly IPlaidItemRepository plaidItemRepository;
    private readonly ConfigProvider configProvider;
    private readonly IStreamlinedTransactionsRepository streamlinedTransactionsRepository;
    private readonly IBankRepository bankRepository;

    public PlaidController(PlaidService plaidService, IPlaidItemRepository plaidItemRepository, ConfigProvider configProvider, IStreamlinedTransactionsRepository streamlinedTransactionsRepository, IBankRepository bankRepository)
    {
        this.configProvider = configProvider;
        this.streamlinedTransactionsRepository = streamlinedTransactionsRepository;
        this.bankRepository = bankRepository;
        this.plaidService = plaidService;
        this.plaidItemRepository = plaidItemRepository;
    }

    [HttpGet("test")]
    public IActionResult Test()
    {
        return Ok("hello there");
    }

    [Authorize]
    [EnableRateLimiting("TransactionsSyncing")]
    [HttpGet("sync-transactions")]
    public async Task<IActionResult> SyncTransactions([FromQuery] string institutionName, [FromQuery] int bankInfoId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

        // get cursor from plaid item
        var plaidItem = await plaidItemRepository.GetPlaidItemAsync(userId, institutionName);

        return await plaidItem.Match<Task<IActionResult>>(
            Some: async (item) =>
            {
                // use cursor in method call
                var transactionSyncResponse = await plaidService.GetAllTransactionsSyncAsync(item.Accesstoken, item.TransactionsCursor);

                // update transactions
                transactionSyncResponse.StreamlinedTransactionDTOs = transactionSyncResponse.StreamlinedTransactionDTOs.Select(st =>
                {
                    // make sure the bankinfo id is set correctly
                    st.BankInfoId = bankInfoId;
                    st.EnvironmentType = configProvider.PlaidEnvironment;
                    return st;
                }).ToList();

                // replace cursor with latest one
                item.TransactionsCursor = transactionSyncResponse.Cursor;
                await plaidItemRepository.UpdatePlaidItemAsync(item);

                var storeTransactionsResponse = await streamlinedTransactionsRepository.StoreTransactionsAsync(transactionSyncResponse.StreamlinedTransactionDTOs);

                return Ok(new GeneralResponse(true, "Latest transactions synced!"));
            },
            None: async () =>
            {
                return await Task.FromResult(
                    BadRequest(new GeneralResponse(false, "Could not find the plaid info to sync your transactions."))
                );
            }
        );
    }

    [Authorize]
    [HttpPost("exchange-public-token")]
    public async Task<IActionResult> ExchangePublicToken([FromBody] PublicTokenRequest request)
    {
        var exchangedResponse = await plaidService.ExchangePublicTokenAsync(request.PublicToken);

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

        var authInfo = await plaidService.GetAuthInfoAsync(exchangedResponse.AccessToken);

        var result = await plaidItemRepository.GetPlaidItemAsync(userId, authInfo.Item.InstitutionName);

        return await result.Match<Task<IActionResult>>(
            Some: async (dto) =>
            {
                // use cursor in method call
                var transactionSyncResponse = await plaidService.GetAllTransactionsSyncAsync(dto.Accesstoken, dto.TransactionsCursor);

                var bankInfoSum = authInfo.Accounts.Sum(acc => acc.Balances.Current.Value);
                var bankInfo = new BankInfoDTO
                {
                    // bankinfoid doesn't matter. We just care about the userid and the bankname for upsert
                    Userid = userId,
                    Bankname = authInfo.Item.InstitutionName,
                    Totalbankbalance = bankInfoSum,
                };

                var upsertBankInfoResponse = await bankRepository.UpsertBankInfoAsync(bankInfo).Match(Left: res => res, Right: res => res);
                
                // replace cursor with latest one
                dto.TransactionsCursor = transactionSyncResponse.Cursor;
                await plaidItemRepository.UpdatePlaidItemAsync(dto);

                // update transactions
                transactionSyncResponse.StreamlinedTransactionDTOs = transactionSyncResponse.StreamlinedTransactionDTOs.Select(st =>
                {
                    // make sure the bankinfo id is set correctly
                    st.BankInfoId = upsertBankInfoResponse.Payload.Bankinfoid;
                    st.EnvironmentType = configProvider.PlaidEnvironment;
                    return st;
                }).ToList();


                // Store the list of queried transactions in streamlinedtransaction table
                var storeTransactionsResponse = await streamlinedTransactionsRepository.StoreTransactionsAsync(transactionSyncResponse.StreamlinedTransactionDTOs).Match(Left: res => res, Right: res => res);


                return await Task.FromResult(Ok(new BankExchangeResponse
                {
                    // Status = upsertBankInfoResponse.Message,
                    Status = storeTransactionsResponse.Message,
                    BankInfo = bankInfo,
                }));
            },
            None: async () =>
            {
                var transactionSyncResponse = await plaidService.GetAllTransactionsSyncAsync(exchangedResponse.AccessToken, null);
                var bankInfoSum = authInfo.Accounts.Sum(acc => acc.Balances.Current.Value);

                var bankInfo = new BankInfoDTO
                {
                    Userid = userId,
                    Bankname = authInfo.Item.InstitutionName,
                    Totalbankbalance = bankInfoSum,
                };

                // Insert bank info into the database
                var storeBankInfoResponse = await bankRepository.UpsertBankInfoAsync(bankInfo).Match(Left: res => res, Right: res => res);

                if (!storeBankInfoResponse.Flag)
                {
                    return BadRequest(new BankExchangeResponse
                    {
                        ErrorMessage = "Couldn't add bank to BankInfo database",
                    });
                }

                transactionSyncResponse.StreamlinedTransactionDTOs = transactionSyncResponse.StreamlinedTransactionDTOs.Select(st =>
                {
                    // make sure the bankinfo id is set correctly
                    st.BankInfoId = storeBankInfoResponse.Payload.Bankinfoid;
                    st.EnvironmentType = configProvider.PlaidEnvironment;
                    return st;
                }).ToList();

                // Store the list of queried transactions in streamlinedtransaction table
                var storeTransactionsResponse = await streamlinedTransactionsRepository.StoreTransactionsAsync(transactionSyncResponse.StreamlinedTransactionDTOs);

                // store access token in db instead of returning it to the client!
                var response = (await plaidItemRepository.StorePlaidItemAsync(new PlaidItemDTO
                {
                    TransactionsCursor = transactionSyncResponse.Cursor,
                    Userid = userId,
                    Accesstoken = exchangedResponse.AccessToken,
                    Institutionname = authInfo.Item.InstitutionName,
                    Datelinked = DateTime.UtcNow,
                })).Match(
                    Left: _ => _,
                    Right: _ => _
                );

                return Ok(new BankExchangeResponse
                {
                    Status = "newly_linked",
                    BankInfo = bankInfo,
                });
            }
        );
    }


    [Authorize]
    [HttpGet("get-link-token/{userId}")]
    public async Task<IActionResult> GetLinkToken(string userId)
    {
        var linkToken = await plaidService.CreateLinkTokenAsync(userId);
        return Ok(linkToken);
    }

}


