
using LanguageExt;
using Server.Models;
using static Server.Models.ServiceResponses;

namespace Server.Repositories;

public interface IPlaidItemRepository
{
    Task<Option<PlaidItemDTO>> GetPlaidItemAsync(int userId, string institutionName);
    Task<IEnumerable<PlaidItemDTO>> GetPlaidItemsAsync(int userId);
    EitherAsync<string, GeneralResponse> StorePlaidItemAsync(PlaidItemDTO? dto);
}
