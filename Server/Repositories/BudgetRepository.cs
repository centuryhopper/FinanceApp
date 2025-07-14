using Server.Contexts;
using Server.Models;

using LanguageExt;
using static LanguageExt.Prelude;
using static Server.Models.ServiceResponses;
using Server.Utils;
using Microsoft.EntityFrameworkCore;

using LstOfSpendings = System.Collections.Generic.IEnumerable<Server.Models.CurrentMonthSpendingByCategoryDTO>;
using Server.Entities;

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

    public EitherAsync<GeneralResponse, GeneralResponse> InitializeBudgetCaps(int userId, int bankInfoId, int categoryId) => TryAsync(async () =>
    {
        var dbTransaction = await financeAppDbContext.Database.BeginTransactionAsync();

        try
        {
            var categories = await financeAppDbContext
                        .Categories
                        .AsNoTracking()
                        .ToListAsync();

            foreach (var category in categories)
            {
                var record = await financeAppDbContext.Budgetcaps.FirstOrDefaultAsync(bc => bc.Bankinfoid == bankInfoId && bc.Categoryid == categoryId && bc.Userid == userId);
                if (record is null)
                {
                    await financeAppDbContext.Budgetcaps.AddAsync(new Budgetcap
                    {
                        Categorybudget = 0,
                        Categoryid = category.Categoryid,
                        Bankinfoid = bankInfoId,
                        Userid = userId,
                    });
                }
            }

            await dbTransaction.CommitAsync();
            return new GeneralResponse(false, "Budget caps initialized!");
        }
        catch (System.Exception ex)
        {
            await dbTransaction.RollbackAsync();
            throw;
        }

    }).ToEither(res => new GeneralResponse(false, res.Message));

    public EitherAsync<GeneralResponse, GeneralResponse> EditBudgetCap(int userId, int bankInfoId, int categoryId)
    {
        throw new NotImplementedException();
    }

    /*
        query to see all total amounts by category:

            select c.name category_name, SUM(amount) from streamlinedtransactions t
            inner join categorytransaction_junc ct on t.streamlinedtransactionsid = ct.streamlinedtransactionsid
            inner join category c on c.categoryid = ct.categoryid
            WHERE EXTRACT(YEAR FROM date) = 2025
            AND EXTRACT(MONTH FROM date) = 6
            group by category_name;

        	
        1	1	Rent/Mortgage	1500
        2	2	Utilities	500
        3	3	Groceries	500
        4	4	Transportation	100
        5	5	Insurance	50
        6	6	Healthcare	50
        7	7	Phone/Internet	100
        8	8	Entertainment	300
        9	9	Other	1000
        10	10	Income	3000

        -- INSERT INTO budgetcaps (categorybudget, categoryid, bankinfoid, userid)
-- VALUES
-- (
--     1500, 1, 2, 1
-- );
-- INSERT INTO budgetcaps (categorybudget, categoryid, bankinfoid, userid)
-- VALUES
-- (
--    500, 2, 2, 1
-- );
-- INSERT INTO budgetcaps (categorybudget, categoryid, bankinfoid, userid)
-- VALUES
-- (
--     500, 3, 2, 1
-- );
-- INSERT INTO budgetcaps (categorybudget, categoryid, bankinfoid, userid)
-- VALUES
-- (
--     100, 4, 2, 1
-- );
-- INSERT INTO budgetcaps (categorybudget, categoryid, bankinfoid, userid)
-- VALUES
-- (
--     50, 5, 2, 1
-- );
-- INSERT INTO budgetcaps (categorybudget, categoryid, bankinfoid, userid)
-- VALUES
-- (
--     50, 6, 2, 1
-- );
-- INSERT INTO budgetcaps (categorybudget, categoryid, bankinfoid, userid)
-- VALUES
-- (
--     100, 7, 2, 1
-- );
-- INSERT INTO budgetcaps (categorybudget, categoryid, bankinfoid, userid)
-- VALUES
-- (
--     300, 8, 2, 1
-- );
-- INSERT INTO budgetcaps (categorybudget, categoryid, bankinfoid, userid)
-- VALUES
-- (
--     1000, 9, 2, 1
-- );
-- INSERT INTO budgetcaps (categorybudget, categoryid, bankinfoid, userid)
-- VALUES
-- (
--     3000, 10, 2, 1
-- );
    */
    public async Task<GeneralResponseWithPayload<LstOfSpendings>> GetCurrentMonthSpendingByCategoriesAsync(int bankInfoId, int userId)
    {
        var bankInfo = await financeAppDbContext.Bankinfos
            .FirstOrDefaultAsync(b => b.Bankinfoid == bankInfoId && b.Userid == userId);

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


        /*
        
            with grouped as (
            SELECT 
            c.name AS category,
            c.categoryid categoryid,
            bc.categorybudget categorybudget,
            sum(st.amount) AS spent
            FROM (
                select * from streamlinedtransactions st1
                where st1.userid = 1 
                AND st1.bankinfoid = 2 
                AND EXTRACT(MONTH FROM st1.date) = 7
                AND EXTRACT(YEAR FROM st1.date) = 2025
            ) AS st
            JOIN categorytransaction_junc cj on cj.streamlinedtransactionsid = st.streamlinedtransactionsid
            RIGHT JOIN category c ON c.categoryid = cj.categoryid
            RIGHT OUTER JOIN budgetcaps bc ON bc.categoryid = c.categoryid 
                AND bc.userid = st.userid 
                AND bc.bankinfoid = st.bankinfoid
            WHERE st.amount > 0
            GROUP BY category, c.categoryid, categorybudget
        )
        select ROW_NUMBER() OVER() - 1 AS id,
        COALESCE(category, c.name) category,
        COALESCE(grouped.categoryid, c.categoryid) categoryid,
        COALESCE(spent, 0) spent,
        bc.categorybudget
        FROM grouped
        -- include everything from these two tables even if there's no intersection with the aggregated table
        RIGHT JOIN category c on c.categoryid = grouped.categoryid
        RIGHT JOIN budgetcaps bc on bc.categoryid = c.categoryid

        */


        var categories = await financeAppDbContext
        .Categories
        .Select(c => c.ToDTO())
        .AsNoTracking()
        .ToListAsync();

        foreach (var category in categories)
        {
            if (spendingByCategories.FirstOrDefault(s => s.CategoryId == category.Categoryid) == null)
            {
                spendingByCategories.Add(new()
                {
                    Id = 0,
                    CategoryId = category.Categoryid,
                    Category = category.Name!,
                    BudgetCap = category.CategoryBudget!.Value,
                    Spent = 0,
                });
            }
        }

        return new GeneralResponseWithPayload<LstOfSpendings>(
            true,
            "Success",
            spendingByCategories
        );
    }
}

