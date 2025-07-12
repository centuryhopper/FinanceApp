using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.Models;
using Server.Repositories;
using Server.Utils;

// TODO: remove categorybudget column from category table and re scaffold to app

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BudgetsController(
        IBudgetRepository budgetRepository
        ) : ControllerBase
    {
        [HttpGet("category-budgets")]
        public async Task<IActionResult> GetCategoryBudgetsAsync()
        {
            var categories = await budgetRepository.GetCategoriesAsync();
            return Ok(categories);
        }

        // http://localhost:5003/api/Budgets/current-month-spending-by-category/Tartan Bank
        [HttpGet("current-month-spending-by-category/{institutionName}")]
        public async Task<IActionResult> GetCurrentMonthSpendingByCategoryAsync(string institutionName)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "1");
            var spendings = await budgetRepository.GetCurrentMonthSpendingByCategoryAsync(institutionName, userId);

            if (!spendings.Flag)
            {
                return BadRequest(new
                {
                    ErrorMessage = "No spendings found"
                });
            }


            return Ok(new
            {
                spendings.Payload,
            });
        }


    }
}
