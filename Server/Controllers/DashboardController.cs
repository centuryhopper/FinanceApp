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
        [HttpGet("transactions/{bankInfoId:int}")]
        public async Task<IActionResult> GetTransactionsAsync(int bankInfoId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var transactions = await streamlinedTransactionsRepository.GetTransactionsAsync(bankInfoId, userId, null);
            return Ok(transactions);
        }

        [HttpPatch("editTransaction")]
        public async Task<IActionResult> EditTransactionsAsync([FromBody] StreamlinedTransactionDTO dto)
        {
            // System.Console.WriteLine("editTransaction called");
            return await streamlinedTransactionsRepository.EditTransactionAsync(dto).Match<IActionResult>(
                Left: (res) => Ok(res),
                Right: (res) => Ok(res)
            );
        }

        // http://localhost:5003/api/Dashboard/monthlySpendings/Tartan Bank
        // [AllowAnonymous]
        [HttpGet("monthlySpendings/{bankInfoId:int}")]
        public async Task<IActionResult> GetMonthlySpendingsAsync(int bankInfoId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

            var monthlySpendings = await streamlinedTransactionsRepository.GetMonthlySpendingsAsync(bankInfoId, userId);
            return Ok(monthlySpendings);
        }
    }
}


/*
select sum(st.amount) totalspent, c.name from streamlinedtransactions st
join categorytransaction_junc cj on cj.streamlinedtransactionsid = st.streamlinedtransactionsid
join category c on c.categoryid = cj.categoryid
where bankinfoid = 3 and c.categoryid = 9
group by c.name;

select *, c.name from streamlinedtransactions st
join categorytransaction_junc cj on cj.streamlinedtransactionsid = st.streamlinedtransactionsid
join category c on c.categoryid = cj.categoryid
where bankinfoid = 3 and c.categoryid = 9 -- AND EXTRACT(MONTH FROM st.date) = 8 AND EXTRACT(YEAR FROM st.date) = 2024

*/