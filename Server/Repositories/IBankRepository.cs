using Server.Models;
using static Server.Models.ServiceResponses;

namespace Server.Repositories;

public interface IBankRepository
{
    Task<GeneralResponseWithPayload<BankInfoDTO?>> GetBankInfoAsync(string institutionName, int userId);

    Task<GeneralResponse> StoreBankInfoAsync(BankInfoDTO bankInfo);
}
