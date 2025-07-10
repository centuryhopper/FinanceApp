using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Models;
using Server.Repositories;
using Server.Utils;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController(
        IStreamlinedTransactionsRepository streamlinedTransactionsRepository) : ControllerBase
    {
        [HttpGet("transactions/{institutionName}")]
        public async Task<IActionResult> GetTransactionsAsync(string institutionName)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var transactions = await streamlinedTransactionsRepository.GetTransactionsAsync(institutionName, userId, null);
            return Ok(transactions);
        }

        [HttpPatch("editTransaction")]
        public async Task<IActionResult> EditTransactionsAsync([FromBody] StreamlinedTransactionDTO dto)
        {
            System.Console.WriteLine("editTransaction called");
            return await streamlinedTransactionsRepository.EditTransactionAsync(dto).Match<IActionResult>(
                Left: (res) => Ok(res),
                Right: (res) => Ok(res)
            );
        }

        // http://localhost:5003/api/Dashboard/monthlySpendings/Tartan Bank
        // [AllowAnonymous]
        [HttpGet("monthlySpendings/{institutionName}")]
        public async Task<IActionResult> GetMonthlySpendingsAsync(string institutionName)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "1");
            var monthlySpendings = await streamlinedTransactionsRepository.GetMonthlySpendingsAsync(institutionName, userId);
            return Ok(monthlySpendings);
        }
    }
}
