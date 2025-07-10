
using LanguageExt;
using Server.Contexts;
using Server.Models;
using static Server.Models.ServiceResponses;
using LanguageExt;
using static LanguageExt.Prelude;
using Server.Utils;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Server.Entities;

namespace Server.Repositories;


public class StreamlinedtransactionsRepository(FinanceAppDbContext financeAppDbContext) : IStreamlinedTransactionsRepository
{
    public async Task<IEnumerable<MonthlySpendingDTO>> GetMonthlySpendingsAsync(string institutionName, int userId)
    {
        var bankInfo = await financeAppDbContext.Bankinfos.FirstOrDefaultAsync(b => b.Bankname == institutionName && b.Userid == userId);

        if (bankInfo == null)
        {
            throw new Exception($"Bank info for institution '{institutionName}' and user ID '{userId}' not found.");
        }

        var result = await financeAppDbContext.Streamlinedtransactions
        .Where(t => t.Userid == userId && t.Bankinfoid == bankInfo.Bankinfoid)
        .Include(t => t.Categories)
        .SelectMany(t => t.Categories.Select(c => new
        {
            t.Date,
            t.Amount,
            Category = c.Name,
        }))
        .GroupBy(x => new
        {
            x.Date.Value.Year,
            x.Date.Value.Month,
            Category = x.Category,
        })
        .Select(g => new
        {
            g.Key.Year,
            g.Key.Month,
            g.Key.Category,
            Total = g.Sum(x => x.Amount.Value)
        })
        .OrderByDescending(x => x.Year)
        .ThenByDescending(x => x.Month)
        .ThenByDescending(x => x.Total)
        .Where(x => x.Total > 0)
        .ToListAsync();

        // Format result in-memory
        var finalResult = result.Select(r => new MonthlySpendingDTO
        {
            Month = $"{r.Year:D4}-{r.Month:D2}", // format like "2025-07"
            CategorySum = new CategorySum
            {
                Category = r.Category,
                Total = r.Total
            }
        }).ToList();

        return finalResult;
    }

    public async Task<IEnumerable<StreamlinedTransactionDTO>> GetTransactionsAsync(string institutionName, int userId, int? numTransactions)
    {
        var bankInfo = await financeAppDbContext.Bankinfos.FirstOrDefaultAsync(b => b.Bankname == institutionName && b.Userid == userId);

        if (bankInfo == null)
        {
            throw new Exception($"Bank info for institution '{institutionName}' and user ID '{userId}' not found.");
        }

        IQueryable<Streamlinedtransaction> query = financeAppDbContext.Streamlinedtransactions
        .Where(t => t.Userid == userId && t.Bankinfoid == bankInfo.Bankinfoid)
        .OrderByDescending(t => t.Date);

        if (numTransactions.HasValue)
        {
            query = query.Take(numTransactions.Value);
        }

        var transactions = await query.Include(t => t.Categories)
        .Select(t => t.ToDTO())
        .ToListAsync();

        return transactions;
    }

    public EitherAsync<string, GeneralResponse> StoreTransactionsAsync(IEnumerable<StreamlinedTransactionDTO> dtos) => TryAsync(async () =>
    {
        if (dtos is null)
        {
            return new GeneralResponse(false, "StreamlinedTransactionDTO cannot be null");
        }
        var categories = await financeAppDbContext
        .Categories
        .AsNoTracking()
        .ToListAsync();
        var transactions = dtos.Select(o => o.ToEntity()).ToList();
        var dbTransaction = await financeAppDbContext.Database.BeginTransactionAsync();

        try
        {
            // store dto in db
            await financeAppDbContext.Streamlinedtransactions.AddRangeAsync(transactions);
            await financeAppDbContext.SaveChangesAsync();

            foreach (var (ent, dto) in transactions.Zip(dtos, (e, d) => (e, d)))
            {
                var categoryToLookFor = dto.Category.ToLower() switch
                {
                    "income" => "income",
                    "medical" => "healthcare",
                    "general_merchandise" => "entertainment",
                    "food_and_drink" => "groceries",
                    "transportation" => "transportation",
                    "rent_and_utilities" => "rent/mortgage",
                    _ => "other"
                };

                // look for category in list
                var category = categories.FirstOrDefault(c => c.Name.ToLower() == categoryToLookFor) ?? throw new Exception($"Category '{categoryToLookFor}' not found in the database.");

                var transactionId = ent.Streamlinedtransactionsid;
                await financeAppDbContext.Database.ExecuteSqlRawAsync(@"
                    INSERT INTO categorytransaction_junc (categoryid, streamlinedtransactionsid)
                    VALUES ({0}, {1})
                ", category.Categoryid, transactionId);
            }

            await dbTransaction.CommitAsync();

            return new GeneralResponse(true, "Streamlined Transaction Added");
        }
        catch (Exception ex)
        {
            await dbTransaction.RollbackAsync();
            throw;
        }

    }).ToEither(ex => ex.Message);
}