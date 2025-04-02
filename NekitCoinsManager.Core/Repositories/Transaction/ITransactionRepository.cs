using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Core.Repositories;

public interface ITransactionRepository : IRepository<Transaction>
{
    Task<IEnumerable<Transaction>> GetUserTransactionsAsync(int userId);
    Task<IEnumerable<Transaction>> GetUserSentTransactionsAsync(int userId);
    Task<IEnumerable<Transaction>> GetUserReceivedTransactionsAsync(int userId);
    Task<IEnumerable<Transaction>> GetRelatedTransactionsAsync(int transactionId);
    Task<bool> HasTransactionsWithCurrencyAsync(int currencyId);
    Task<IEnumerable<Transaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<decimal> GetUserBalanceInCurrencyAsync(int userId, int currencyId);
} 