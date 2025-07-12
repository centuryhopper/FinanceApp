using Server.Contexts;
using Server.Models;

using LanguageExt;
using static LanguageExt.Prelude;
using static Server.Models.ServiceResponses;
using Server.Services;
using Server.Utils;
using Microsoft.EntityFrameworkCore;

using LstOfSpendings = System.Collections.Generic.IEnumerable<Server.Models.CurrentMonthSpendingByCategoryDTO>;

namespace Server.Repositories;

public class BudgetRepository(FinanceAppDbContext financeAppDbContext) : IBudgetRepository
{

    public async Task<IEnumerable<CategoryDTO>> GetCategoriesAsync()
    {
        var categories = await financeAppDbContext
            .Categories
            .Select(c => c.ToDTO())
            .AsNoTracking()
            .ToListAsync();

        return categories;
    }

    /*
        query to see all total amounts by category:

            select c.name category_name, SUM(amount) from streamlinedtransactions t
            inner join categorytransaction_junc ct on t.streamlinedtransactionsid = ct.streamlinedtransactionsid
            inner join category c on c.categoryid = ct.categoryid
            WHERE EXTRACT(YEAR FROM date) = 2025
            AND EXTRACT(MONTH FROM date) = 6
            group by category_name;
    */

    public async Task<GeneralResponseWithPayload<LstOfSpendings>> GetCurrentMonthSpendingByCategoryAsync(string institutionName, int userId)
    {
        var bankInfo = await financeAppDbContext.Bankinfos
            .FirstOrDefaultAsync(b => b.Bankname == institutionName && b.Userid == userId);

        if (bankInfo == null)
        {
            return new GeneralResponseWithPayload<LstOfSpendings>(
                false,
                "Couldn't find your bank information",
                Enumerable.Empty<CurrentMonthSpendingByCategoryDTO>()
            );
        }

        var currentMonth = DateTime.Now.Month;
        var currentYear = DateTime.Now.Year;

        var spendingByCategory = await financeAppDbContext.Streamlinedtransactions
            .Where(t => t.Userid == userId && t.Bankinfoid == bankInfo.Bankinfoid)
            .Where(t => t.Date.HasValue &&
                        t.Date.Value.Month == currentMonth &&
                        t.Date.Value.Year == currentYear)
            .Include(t => t.Categories)
            .SelectMany(t => t.Categories.Select(c => new
            {
                CategoryId = c.Categoryid,
                CategoryName = c.Name,
                Amount = t.Amount ?? 0,
                BudgetCap = c.CategoryBudget,
                Id = 0
            }))
            .Where(t => t.BudgetCap.HasValue)
            .GroupBy(t => new
            {
                t.CategoryId,
                t.CategoryName,
                t.Id,
                t.BudgetCap,
            })
            .Select(g => new CurrentMonthSpendingByCategoryDTO
            {
                CategoryId = g.Key.CategoryId,
                Category = g.Key.CategoryName!,
                Id = g.Key.Id,
                BudgetCap = g.Key.BudgetCap.Value,
                Spent = g.Sum(t => t.Amount)
            })
            .Where(o => o.Spent > 0)
            .ToListAsync();

        var categories = await financeAppDbContext
        .Categories
        .Select(c => c.ToDTO())
        .AsNoTracking()
        .ToListAsync();

        foreach (var category in categories)
        {
            if (spendingByCategory.FirstOrDefault(s => s.CategoryId == category.Categoryid) == null)
            {
                spendingByCategory.Add(new()
                {
                    CategoryId = category.Categoryid,
                    Category = category.Name!,
                    Id = 0,
                    BudgetCap = category.CategoryBudget!.Value,
                    Spent = 0,
                });
            }
        }

        for (int i = 0; i < spendingByCategory.Count; i++)
        {
            spendingByCategory[i].Id = i;
        }

        return new GeneralResponseWithPayload<LstOfSpendings>(
                    true,
                    "Success",
                    spendingByCategory
                );
    }



}

