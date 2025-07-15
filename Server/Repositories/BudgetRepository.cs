using Server.Contexts;
using Server.Models;

using LanguageExt;
using static LanguageExt.Prelude;
using static Server.Models.ServiceResponses;
using Server.Utils;
using Microsoft.EntityFrameworkCore;

using LstOfSpendings = System.Collections.Generic.IEnumerable<Server.Models.CurrentMonthSpendingByCategoryDTO>;
using Server.Entities;
using Newtonsoft.Json;

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

    public EitherAsync<GeneralResponse, GeneralResponse> InitializeBudgetCaps(int userId, int bankInfoId) => TryAsync(async () =>
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
                var record = await financeAppDbContext.Budgetcaps.FirstOrDefaultAsync(bc =>
                    bc.Bankinfoid == bankInfoId &&
                    bc.Categoryid == category.Categoryid &&
                    bc.Userid == userId);
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
            return new GeneralResponse(true, "Budget caps initialized!");
        }
        catch (System.Exception ex)
        {
            await dbTransaction.RollbackAsync();
            throw;
        }

    }).ToEither(res => new GeneralResponse(false, res.Message));

    public EitherAsync<GeneralResponse, GeneralResponse> EditBudgetCap(BudgetCapDTO dto) => TryAsync(async () =>
    {
        var record = await financeAppDbContext.Budgetcaps.FirstOrDefaultAsync(bc => bc.Bankinfoid == dto.Bankinfoid && bc.Categoryid == dto.Categoryid && bc.Userid == dto.Userid);

        if (record is null)
        {
            throw new Exception("Budget cap record not found.");
        }

        record.Categorybudget = dto.Categorybudget;
        await financeAppDbContext.SaveChangesAsync();

        return new GeneralResponse(true, "Budget cap record updated!");

    }).ToEither(ex => new GeneralResponse(false, ex.Message));

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
    public EitherAsync<GeneralResponseWithPayload<LstOfSpendings>, GeneralResponseWithPayload<LstOfSpendings>> GetCurrentMonthSpendingByCategoriesAsync(int bankInfoId, int userId) => TryAsync(async () =>
    {

        var bankInfo = await financeAppDbContext.Bankinfos
            .FirstOrDefaultAsync(b => b.Bankinfoid == bankInfoId && b.Userid == userId);

        if (bankInfo == null)
        {
            throw new Exception("Couldn't find your bank information");
        }

        var currentMonth = DateTime.Now.Month;
        var currentYear = DateTime.Now.Year;

        var spendingByCategories = await financeAppDbContext.Database.SqlQueryRaw<CurrentMonthSpendingByCategoryDTO>(@"
                WITH grouped AS (
                    SELECT 
                    c.name AS category,
                    c.categoryid categoryid,
                    bc.categorybudget categorybudget,
                    SUM(st.amount) AS spent
                    FROM (
                        SELECT * FROM streamlinedtransactions st1
                        WHERE st1.userid = {0}
                        AND st1.bankinfoid = {1}
                        AND EXTRACT(MONTH FROM st1.date) = {2}
                        AND EXTRACT(YEAR FROM st1.date) = {3}
                    ) AS st
                    JOIN categorytransaction_junc cj on cj.streamlinedtransactionsid = st.streamlinedtransactionsid
                    RIGHT JOIN category c ON c.categoryid = cj.categoryid
                    RIGHT OUTER JOIN budgetcaps bc ON bc.categoryid = c.categoryid 
                        AND bc.userid = st.userid 
                        AND bc.bankinfoid = st.bankinfoid
                    WHERE st.amount > 0
                    GROUP BY category, c.categoryid, categorybudget
                )
                SELECT ROW_NUMBER() OVER() - 1 AS Id,
                COALESCE(category, c.name) Category,
                COALESCE(grouped.categoryid, c.categoryid) CategoryId,
                COALESCE(spent, 0) Spent,
                bc.categorybudget CategoryBudget
                FROM grouped
                -- include everything from these two tables even if there's no intersection with the aggregated table
                RIGHT JOIN category c on c.categoryid = grouped.categoryid
                RIGHT JOIN budgetcaps bc on bc.categoryid = c.categoryid
                ORDER BY CategoryId
        ",
        userId,
        bankInfoId,
        currentMonth,
        currentYear)
        .ToListAsync();

        return new GeneralResponseWithPayload<LstOfSpendings>(
            true,
            "Success",
            spendingByCategories
        );

    }).ToEither(ex => new GeneralResponseWithPayload<LstOfSpendings>(false, ex.Message, Enumerable.Empty<CurrentMonthSpendingByCategoryDTO>()));
}

