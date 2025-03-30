using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NekitCoinsManager.Core.Data;
using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Core.Services;

public class CurrencyService : ICurrencyService
{
    private readonly AppDbContext _dbContext;

    public CurrencyService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Currency>> GetCurrenciesAsync()
    {
        return await _dbContext.Currencies
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Currency?> GetCurrencyByIdAsync(int id)
    {
        return await _dbContext.Currencies
            .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);
    }

    public async Task<Currency?> GetCurrencyByCodeAsync(string code)
    {
        return await _dbContext.Currencies
            .FirstOrDefaultAsync(c => c.Code.Equals(code) && c.IsActive);
    }

    public async Task<(bool success, string? error)> AddCurrencyAsync(Currency currency)
    {
        if (string.IsNullOrWhiteSpace(currency.Name))
        {
            return (false, "Название валюты не может быть пустым");
        }

        if (string.IsNullOrWhiteSpace(currency.Code))
        {
            return (false, "Код валюты не может быть пустым");
        }

        if (string.IsNullOrWhiteSpace(currency.Symbol))
        {
            return (false, "Символ валюты не может быть пустым");
        }

        var existingCurrency = await GetCurrencyByCodeAsync(currency.Code);
        if (existingCurrency != null)
        {
            return (false, "Валюта с таким кодом уже существует");
        }

        currency.LastUpdateTime = DateTime.UtcNow;
        currency.IsActive = true;

        _dbContext.Currencies.Add(currency);
        await _dbContext.SaveChangesAsync();

        return (true, null);
    }

    public async Task<(bool success, string? error)> UpdateCurrencyAsync(Currency currency)
    {
        var existingCurrency = await GetCurrencyByIdAsync(currency.Id);
        if (existingCurrency == null)
        {
            return (false, "Валюта не найдена");
        }

        if (!string.IsNullOrWhiteSpace(currency.Name))
        {
            existingCurrency.Name = currency.Name;
        }

        if (!string.IsNullOrWhiteSpace(currency.Symbol))
        {
            existingCurrency.Symbol = currency.Symbol;
        }

        existingCurrency.LastUpdateTime = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
        return (true, null);
    }

    public async Task<(bool success, string? error)> DeleteCurrencyAsync(int id)
    {
        var currency = await GetCurrencyByIdAsync(id);
        if (currency == null)
        {
            return (false, "Валюта не найдена");
        }

        // Проверяем наличие транзакций с этой валютой
        var hasTransactions = await _dbContext.Transactions
            .AnyAsync(t => t.CurrencyId == id);
        if (hasTransactions)
        {
            return (false, "Невозможно удалить валюту, так как с ней есть связанные транзакции");
        }

        // Проверяем наличие балансов пользователей в этой валюте
        var hasBalances = await _dbContext.UserBalances
            .AnyAsync(b => b.CurrencyId == id);
        if (hasBalances)
        {
            return (false, "Невозможно удалить валюту, так как есть пользователи с балансом в этой валюте");
        }

        currency.IsActive = false;
        currency.LastUpdateTime = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
        return (true, null);
    }

    public async Task<(bool success, string? error)> UpdateExchangeRateAsync(int currencyId, decimal newRate)
    {
        var currency = await GetCurrencyByIdAsync(currencyId);
        if (currency == null)
        {
            return (false, "Валюта не найдена");
        }

        if (newRate <= 0)
        {
            return (false, "Курс обмена должен быть больше нуля");
        }

        currency.ExchangeRate = newRate;
        currency.LastUpdateTime = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
        return (true, null);
    }
} 