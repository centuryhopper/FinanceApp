using Microsoft.AspNetCore.Mvc;
using Server.Services;
using Newtonsoft.Json;
using Going.Plaid.Auth;
using Server.Repositories;
using Server.Models;
using System.Security.Claims;
using static Server.Models.ServiceResponses;
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
    public async Task<IActionResult> SyncTransactions([FromQuery] string institutionName)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

        // get cursor from plaid item
        var plaidItem = await plaidItemRepository.GetPlaidItemAsync(userId, institutionName);

        return await plaidItem.Match<Task<IActionResult>>(
            Some: async (item) =>
            {
                // use cursor in method call
                var transactionSyncResponse = await plaidService.GetAllTransactionsSyncAsync(item.Accesstoken, item.TransactionsCursor);

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
                return await Task.FromResult(Ok(new
                {
                    status = "already_linked",
                }));
            },
            None: async () =>
            {
                var transactionSyncResponse = await plaidService.GetAllTransactionsSyncAsync(exchangedResponse.AccessToken, null);

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
                    Left: msg => new GeneralResponse(false, msg),
                    Right: _ => _
                );

                var bankInfoSum = authInfo.Accounts.Sum(acc => acc.Balances.Current.Value);

                var bankInfo = new BankInfoDTO
                {
                    Userid = userId,
                    Bankname = authInfo.Item.InstitutionName,
                    Totalbankbalance = bankInfoSum,
                };

                // Insert bank info into the database
                var storeBankInfoResponse = await bankRepository.StoreBankInfoAsync(bankInfo);

                return Ok(new
                {
                    status = "newly_linked",
                    bankInfo,
                    name = authInfo.Item.InstitutionName,
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


