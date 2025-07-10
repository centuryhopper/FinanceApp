
using LanguageExt;
using Server.Models;
using static Server.Models.ServiceResponses;

namespace Server.Repositories;

public interface IStreamlinedTransactionsRepository
{
    Task<IEnumerable<MonthlySpendingDTO>> GetMonthlySpendingsAsync(string institutionName, int userId);
    Task<IEnumerable<StreamlinedTransactionDTO>> GetTransactionsAsync(string institutionName, int userId, int? numTransactions);
    EitherAsync<string, GeneralResponse> StoreTransactionsAsync(IEnumerable<StreamlinedTransactionDTO> dto);
}
