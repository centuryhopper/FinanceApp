
using Server.Contexts;
using Server.Models;

using LanguageExt;
using static LanguageExt.Prelude;
using static Server.Models.ServiceResponses;
using Server.Utils;

namespace Server.Repositories;

public class PlaidItemRepository(FinanceAppDbContext financeAppDbContext) : IPlaidItemRepository
{
    public EitherAsync<string, GeneralResponse> StorePlaidItemAsync(PlaidItemDTO? dto) => TryAsync(async () =>
    {
        if (dto is null)
        {
            return new GeneralResponse(false, "PlaidItemDTO cannot be null");
        }

        // store dto in db
        await financeAppDbContext.AddAsync(dto.ToEntity());
        await financeAppDbContext.SaveChangesAsync();

        return new GeneralResponse(true, "Plaid item added");

    }).ToEither(ex => ex.Message);
}