
using LanguageExt;
using Server.Contexts;
using Server.Models;
using static Server.Models.ServiceResponses;
using LanguageExt;
using static LanguageExt.Prelude;
using Server.Utils;
using Microsoft.EntityFrameworkCore;

namespace Server.Repositories;


public class StreamlinedtransactionsRepository(FinanceAppDbContext financeAppDbContext) : IStreamlinedTransactionsRepository
{
    public async Task<IEnumerable<StreamlinedTransactionDTO>> GetTransactionsAsync(string institutionName, int userId, int numTransactions)
    {
        var bankInfo = await financeAppDbContext.Bankinfos.FirstOrDefaultAsync(b => b.Bankname == institutionName && b.Userid == userId);

        if (bankInfo == null)
        {
            throw new Exception($"Bank info for institution '{institutionName}' and user ID '{userId}' not found.");
        }

        var transactions = await financeAppDbContext.Streamlinedtransactions
        .Where(t => t.Userid == userId && t.Bankinfoid == bankInfo.Bankinfoid)
        .Take(numTransactions)
        .Include(t => t.Categories)
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