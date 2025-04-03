using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Core.Repositories;

public interface ICurrencyRepository : IRepository<Currency>
{
    Task<Currency?> GetByCodeAsync(string code);
    Task<IEnumerable<Currency>> GetActiveCurrenciesAsync();
    Task<bool> IsCodeUniqueAsync(string code, int? excludeId = null);
    Task<(bool isValid, ErrorCode? error)> ValidateExchangeRateAsync(decimal rate);
    Task<(bool canDelete, ErrorCode? error)> CanDeleteAsync(int currencyId, ITransactionRepository transactionRepository, IUserBalanceRepository userBalanceRepository);
} 