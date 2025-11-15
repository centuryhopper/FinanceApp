using Server.Contexts;
using Shared.Models;

using LanguageExt;
using static LanguageExt.Prelude;
using static Shared.Models.ServiceResponses;
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

    public async Task<GeneralResponseWithPayload<BankInfoDTO>> StoreBankInfoAsync(BankInfoDTO bankInfo)
    {
        try
        {
            var bankEntity = bankInfo.ToEntity();
            await financeAppDbContext.Bankinfos.AddAsync(bankEntity);
            await financeAppDbContext.SaveChangesAsync();

            return new GeneralResponseWithPayload<BankInfoDTO>(true, "Bank info stored successfully.", bankEntity.ToDTO());
        }
        catch (System.Exception ex)
        {
            return new GeneralResponseWithPayload<BankInfoDTO>(false, ex.Message, null!);
        }
    }

    public EitherAsync<GeneralResponseWithPayload<BankInfoDTO?>, GeneralResponseWithPayload<BankInfoDTO>> UpsertBankInfoAsync(BankInfoDTO bankInfo) => TryAsync(async () =>
    {
        var bankEntity = await financeAppDbContext.Bankinfos.FirstOrDefaultAsync(b => b.Userid == bankInfo.Userid && b.Bankname == bankInfo.Bankname);
        if (bankEntity == null)
        {
            var response = await StoreBankInfoAsync(bankInfo);
            return new GeneralResponseWithPayload<BankInfoDTO>(true, response.Message, response.Payload);
        }
        else
        {
            bankEntity.Totalbankbalance = bankInfo.Totalbankbalance;
            await financeAppDbContext.SaveChangesAsync();

            return new GeneralResponseWithPayload<BankInfoDTO>(true, "Bank info upserted successfully.", bankEntity.ToDTO());
        }

    }).ToEither(ex => new GeneralResponseWithPayload<BankInfoDTO?>(false, ex.Message, null));

}

