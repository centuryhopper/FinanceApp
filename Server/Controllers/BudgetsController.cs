using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Server.Models;
using Server.Repositories;
using Server.Utils;


namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BudgetsController(
        IBudgetRepository budgetRepository
        ) : ControllerBase
    {

        [HttpGet("init-budgets/{bankInfoId:int}")]
        public async Task<IActionResult> InitBudgetsAsync(int bankInfoId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

            return (await budgetRepository.InitializeBudgetCaps(userId, bankInfoId)).Match<IActionResult>(
                Left: res => BadRequest(res),
                Right: res => Ok(res)
            );
        }

        [HttpGet("current-month-spending-by-category/{bankInfoId:int}")]
        public async Task<IActionResult> GetCurrentMonthSpendingByCategoryAsync(int bankInfoId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

            return (await budgetRepository.GetCurrentMonthSpendingByCategoriesAsync(bankInfoId, userId)).Match<IActionResult>(
                Left: res => BadRequest(new
                {
                    ErrorMessage = res.Message,
                }),
                Right: res => Ok(new
                {
                    res.Payload
                })
            );
        }

        [HttpPatch("edit-budgetcap")]
        public async Task<IActionResult> EditBudgetCap(
            [FromQuery] int categoryId,
            [FromQuery] int bankInfoId,
            [FromQuery] int budgetCap)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

            return (await budgetRepository.EditBudgetCap(new()
            {
                Categorybudget = budgetCap,
                Categoryid = categoryId,
                Bankinfoid = bankInfoId,
                Userid = userId,
            })).Match<IActionResult>(
                Left: (res) => BadRequest(res),
                Right: (res) => Ok(res)
            );
        }


    }
}
