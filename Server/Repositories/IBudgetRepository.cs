
using LanguageExt;
using Shared.Models;
using static Shared.Models.ServiceResponses;
using LstOfSpendings = System.Collections.Generic.IEnumerable<Shared.Models.CurrentMonthSpendingByCategoryDTO>;


namespace Server.Repositories;

public interface IBudgetRepository
{
    EitherAsync<GeneralResponse, GeneralResponse> InitializeBudgetCaps(int userId, int bankInfoId);
    EitherAsync<GeneralResponse, GeneralResponse> EditBudgetCap(BudgetCapDTO dto);
    EitherAsync<GeneralResponseWithPayload<LstOfSpendings>, GeneralResponseWithPayload<LstOfSpendings>> GetCurrentMonthSpendingByCategoriesAsync(int bankInfoId, int userId);
}
