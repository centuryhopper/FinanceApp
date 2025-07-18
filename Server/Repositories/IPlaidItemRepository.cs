
using LanguageExt;
using Server.Models;
using static Server.Models.ServiceResponses;

namespace Server.Repositories;

public interface IPlaidItemRepository
{
    Task<Option<PlaidItemDTO>> GetPlaidItemAsync(int userId, string institutionName);
    /// <summary>
    /// Get all plaid-linked banks associated to the current user
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<IEnumerable<PlaidItemDTO>> GetPlaidItemsAsync(int userId);
    EitherAsync<GeneralResponse, GeneralResponse> StorePlaidItemAsync(PlaidItemDTO? dto);
    EitherAsync<GeneralResponse, GeneralResponse> UpdatePlaidItemAsync(PlaidItemDTO? dto);
}
