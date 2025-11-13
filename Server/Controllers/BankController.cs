using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using Server.Repositories;
using Server.Utils;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BankController(
        IBankRepository bankRepository,
        IPlaidItemRepository plaidItemRepository,
        ConfigProvider configProvider,
        IStreamlinedTransactionsRepository streamlinedTransactionsRepository) : ControllerBase
    {
        [HttpGet("get-banks")]
        public async Task<IActionResult> GetBanks()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var plaidItems = await plaidItemRepository.GetPlaidItemsAsync(userId);
            List<BankInfoDTO?> bankInfos = [];

            foreach (var plaidItem in plaidItems)
            {
                if (!plaidItem.Accesstoken.Contains(configProvider.PlaidEnvironment))
                {
                    continue;
                }
                var bank = await bankRepository.GetBankInfoAsync(plaidItem.Institutionname, userId).Match(Left: res => res, Right: res => res);

                if (!bank.Flag)
                {
                    continue;
                }

                bankInfos.Add(bank.Payload);
            }

            return Ok(bankInfos);
        }

        [HttpGet("recent-transactions/{bankInfoId:int}")]
        public async Task<IActionResult> GetTransactionsAsync(int bankInfoId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var transactions = await streamlinedTransactionsRepository.GetTransactionsAsync(bankInfoId, userId, 5);
            return Ok(transactions);
        }


    }
}
