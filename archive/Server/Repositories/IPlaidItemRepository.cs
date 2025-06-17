
using LanguageExt;
using Server.Models;
using static Server.Models.ServiceResponses;

namespace Server.Repositories;

public interface IPlaidItemRepository
{
    EitherAsync<string, GeneralResponse> StorePlaidItemAsync(PlaidItemDTO? dto);
}
