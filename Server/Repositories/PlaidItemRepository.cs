
using Server.Contexts;
using Shared.Models;

using LanguageExt;
using static LanguageExt.Prelude;
using static Shared.Models.ServiceResponses;
using Server.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;

namespace Server.Repositories;

public class PlaidItemRepository(FinanceAppDbContext financeAppDbContext, EncryptionContext encryptionContext) : IPlaidItemRepository
{
    public EitherAsync<GeneralResponse, GeneralResponse> StorePlaidItemAsync(PlaidItemDTO? dto) => TryAsync(async () =>
    {
        if (dto is null)
        {
            return new GeneralResponse(false, "PlaidItemDTO cannot be null");
        }

        // store dto in db
        await financeAppDbContext.Plaiditems.AddAsync(dto.ToEntity(encryptionContext));
        await financeAppDbContext.SaveChangesAsync();

        return new GeneralResponse(true, "Plaid item added");

    }).ToEither(ex => new GeneralResponse(false, ex.Message));


    public async Task<IEnumerable<PlaidItemDTO>> GetPlaidItemsAsync(int userId)
    {
        return await financeAppDbContext.Plaiditems
        .Where(item => item.Userid == userId)
        .Select(item => item.ToDTO(encryptionContext))
        .AsNoTracking()
        .ToListAsync();
    }

    public async Task<Option<PlaidItemDTO>> GetPlaidItemAsync(int userId, string institutionName)
    {
        var entity = await financeAppDbContext.Plaiditems
        .Where(item =>
            item.Userid == userId && item.Institutionname.ToLower() == institutionName.ToLower()
        )
        .FirstOrDefaultAsync();

        return entity is null ? None : Some(entity.ToDTO(encryptionContext));
    }

    public EitherAsync<GeneralResponse, GeneralResponse> UpdatePlaidItemAsync(PlaidItemDTO? dto) => TryAsync(async () =>
    {
        if (dto is null)
        {
            return new GeneralResponse(false, "PlaidItemDTO cannot be null");
        }

        // store dto in db
        var entity = await financeAppDbContext.Plaiditems.FindAsync(dto.Plaiditemid);
        entity!.TransactionsCursor = dto.TransactionsCursor;
        await financeAppDbContext.SaveChangesAsync();

        return new GeneralResponse(true, "Plaid item added");

    }).ToEither(ex => new GeneralResponse(false, ex.Message));
}