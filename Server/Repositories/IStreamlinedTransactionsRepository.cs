
using LanguageExt;
using Shared.Models;
using static Shared.Models.ServiceResponses;

namespace Server.Repositories;

public interface IStreamlinedTransactionsRepository
{
    Task<IEnumerable<MonthlySpendingDTO>> GetMonthlySpendingsAsync(int bankInfoId, int userId);
    Task<IEnumerable<StreamlinedTransactionDTO>> GetTransactionsAsync(int bankInfoId, int userId, int? numTransactions);
    EitherAsync<GeneralResponse, GeneralResponse> StoreTransactionsAsync(IEnumerable<StreamlinedTransactionDTO> dto);
    EitherAsync<GeneralResponse, GeneralResponse> EditTransactionAsync(StreamlinedTransactionDTO dto);
}
