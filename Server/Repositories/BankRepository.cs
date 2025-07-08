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
    public async Task<GeneralResponseWithPayload<BankInfoDTO?>> GetBankInfoAsync(string institutionName, int userId)
    {
        try
        {
            var bankInfo = await financeAppDbContext.Bankinfos.FirstOrDefaultAsync(b => b.Userid == userId && b.Bankname == institutionName);

            return new GeneralResponseWithPayload<BankInfoDTO?>(true, "success", bankInfo.ToDTO());

        }
        catch (System.Exception ex)
        {
            return new GeneralResponseWithPayload<BankInfoDTO?>(false, ex.Message, null);
        }
    }

    public async Task<GeneralResponse> StoreBankInfoAsync(BankInfoDTO bankInfo)
    {
        try
        {
            await financeAppDbContext.Bankinfos.AddAsync(bankInfo.ToEntity());
            await financeAppDbContext.SaveChangesAsync();

            return new GeneralResponse(true, "Bank info stored successfully.");
        }
        catch (System.Exception ex)
        {
            return new GeneralResponse(false, ex.Message);
        }
    }
}

