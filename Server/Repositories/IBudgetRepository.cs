
using Server.Models;
using static Server.Models.ServiceResponses;
using LstOfSpendings = System.Collections.Generic.IEnumerable<Server.Models.CurrentMonthSpendingByCategoryDTO>;


namespace Server.Repositories;

public interface IBudgetRepository
{
    Task<IEnumerable<CategoryDTO>> GetCategoriesAsync();
    Task<GeneralResponseWithPayload<LstOfSpendings>> GetCurrentMonthSpendingByCategoryAsync(string institutionName, int userId);
}
