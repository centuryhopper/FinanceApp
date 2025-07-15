using LanguageExt;
using Server.Models;
using static Server.Models.ServiceResponses;

namespace Server.Repositories;

public interface IBankRepository
{
    EitherAsync<GeneralResponseWithPayload<BankInfoDTO?>, GeneralResponseWithPayload<BankInfoDTO>> GetBankInfoAsync(string institutionName, int userId);

    EitherAsync<GeneralResponseWithPayload<BankInfoDTO?>, GeneralResponseWithPayload<BankInfoDTO>> StoreBankInfoAsync(BankInfoDTO bankInfo);
}
