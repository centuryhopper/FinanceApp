using Server.Contexts;
using Server.Models;

using LanguageExt;
using static LanguageExt.Prelude;
using static Server.Models.ServiceResponses;
using Server.Services;
using Server.Utils;
using Microsoft.EntityFrameworkCore;

namespace Server.Repositories;

public class BankRepository(FinanceAppDbContext financeAppDbContext) : IBankRepository
{
    public EitherAsync<GeneralResponseWithPayload<BankInfoDTO?>, GeneralResponseWithPayload<BankInfoDTO>> GetBankInfoAsync(string institutionName, int userId) => TryAsync(async () =>
    {
        var bankInfo = await financeAppDbContext.Bankinfos.FirstOrDefaultAsync(b => b.Userid == userId && b.Bankname == institutionName);
        if (bankInfo == null)
        {
            throw new Exception("bank info not found");
        }

        return new GeneralResponseWithPayload<BankInfoDTO>(true, "success", bankInfo.ToDTO());

    }).ToEither(ex => new GeneralResponseWithPayload<BankInfoDTO?>(false, ex.Message, null));

    public EitherAsync<GeneralResponseWithPayload<BankInfoDTO?>, GeneralResponseWithPayload<BankInfoDTO>> StoreBankInfoAsync(BankInfoDTO bankInfo) => TryAsync(async () =>
    {
        var bankEntity = bankInfo.ToEntity();
        await financeAppDbContext.Bankinfos.AddAsync(bankEntity);
        await financeAppDbContext.SaveChangesAsync();

        return new GeneralResponseWithPayload<BankInfoDTO>(true, "Bank info stored successfully.", bankEntity.ToDTO());
    }).ToEither(ex => new GeneralResponseWithPayload<BankInfoDTO?>(false, ex.Message, null));

}

