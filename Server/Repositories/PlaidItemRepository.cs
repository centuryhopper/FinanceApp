
using Server.Contexts;
using Server.Models;

using LanguageExt;
using static LanguageExt.Prelude;
using static Server.Models.ServiceResponses;
using Server.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;

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


    public async Task<IEnumerable<PlaidItemDTO>> GetPlaidItemsAsync(int userId)
    {
        return await financeAppDbContext.Plaiditems
        .Where(item => item.Userid == userId)
        .Select(item => item.ToDTO())
        .AsNoTracking()
        .ToListAsync();
    }

    public async Task<Option<PlaidItemDTO>> GetPlaidItemAsync(int userId, string institutionName)
    {
        var entity = await financeAppDbContext.Plaiditems.Where(item => item.Userid == userId && item.Institutionname == institutionName).FirstOrDefaultAsync();

        return entity is null ? None : Some(entity.ToDTO());
    }
}