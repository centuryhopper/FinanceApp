
using LanguageExt;
using Server.Models;
using static Server.Models.ServiceResponses;
using LstOfSpendings = System.Collections.Generic.IEnumerable<Server.Models.CurrentMonthSpendingByCategoryDTO>;


namespace Server.Repositories;

public interface IBudgetRepository
{
    EitherAsync<GeneralResponse, GeneralResponse> InitializeBudgetCaps(int userId, int bankInfoId, int categoryId);
    EitherAsync<GeneralResponse, GeneralResponse> EditBudgetCap(int userId, int bankInfoId, int categoryId);
    Task<IEnumerable<CategoryDTO>> GetCategoriesAsync();
    Task<GeneralResponseWithPayload<LstOfSpendings>> GetCurrentMonthSpendingByCategoriesAsync(int bankInfoId, int userId);
}
