using System.Collections.Generic;
using System.Threading.Tasks;
using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Core.Services;

public interface ICurrencyService
{
    Task<IEnumerable<Currency>> GetCurrenciesAsync();
    Task<Currency?> GetCurrencyByIdAsync(int id);
    Task<Currency?> GetCurrencyByCodeAsync(string code);
    Task<(bool success, string? error)> AddCurrencyAsync(Currency currency);
    Task<(bool success, string? error)> UpdateCurrencyAsync(Currency currency);
    Task<(bool success, string? error)> DeleteCurrencyAsync(int id);
    Task<(bool success, string? error)> UpdateExchangeRateAsync(int currencyId, decimal newRate);
} 