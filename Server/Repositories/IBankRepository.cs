using LanguageExt;
using Shared.Models;
using static Shared.Models.ServiceResponses;

namespace Server.Repositories;

public interface IBankRepository
{
    EitherAsync<GeneralResponseWithPayload<BankInfoDTO?>, GeneralResponseWithPayload<BankInfoDTO>> GetBankInfoAsync(string institutionName, int userId);

    EitherAsync<GeneralResponseWithPayload<BankInfoDTO?>, GeneralResponseWithPayload<BankInfoDTO>> StoreBankInfoAsync(BankInfoDTO bankInfo);
}
