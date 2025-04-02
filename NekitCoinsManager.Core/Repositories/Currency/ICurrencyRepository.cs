using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Core.Repositories;

public interface ICurrencyRepository : IRepository<Currency>
{
    Task<Currency?> GetByCodeAsync(string code);
    Task<IEnumerable<Currency>> GetActiveCurrenciesAsync();
    Task<Currency?> GetDefaultCurrencyAsync();
    Task<bool> IsCodeUniqueAsync(string code, int? excludeId = null);
} 