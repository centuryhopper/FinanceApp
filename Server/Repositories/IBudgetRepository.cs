
using LanguageExt;
using Server.Models;
using static Server.Models.ServiceResponses;
using LstOfSpendings = System.Collections.Generic.IEnumerable<Server.Models.CurrentMonthSpendingByCategoryDTO>;


namespace Server.Repositories;

public interface IBudgetRepository
{
    Task<IEnumerable<CategoryDTO>> GetCategoriesAsync();
    EitherAsync<GeneralResponse, GeneralResponse> InitializeBudgetCaps(int userId, int bankInfoId);
    EitherAsync<GeneralResponse, GeneralResponse> EditBudgetCap(BudgetCapDTO dto);
    EitherAsync<GeneralResponseWithPayload<LstOfSpendings>, GeneralResponseWithPayload<LstOfSpendings>> GetCurrentMonthSpendingByCategoriesAsync(int bankInfoId, int userId);
}
