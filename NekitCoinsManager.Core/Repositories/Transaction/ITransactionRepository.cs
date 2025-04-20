using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Core.Repositories;

public interface ITransactionRepository : IRepository<Transaction>
{
    Task<bool> HasTransactionsWithCurrencyAsync(int currencyId);
} 