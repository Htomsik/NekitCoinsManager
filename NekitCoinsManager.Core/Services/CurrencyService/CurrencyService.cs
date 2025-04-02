using NekitCoinsManager.Core.Models;
using NekitCoinsManager.Core.Repositories;

namespace NekitCoinsManager.Core.Services;

public class CurrencyService : ICurrencyService
{
    private readonly ICurrencyRepository _currencyRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUserBalanceRepository _userBalanceRepository;

    public CurrencyService(
        ICurrencyRepository currencyRepository,
        ITransactionRepository transactionRepository,
        IUserBalanceRepository userBalanceRepository)
    {
        _currencyRepository = currencyRepository;
        _transactionRepository = transactionRepository;
        _userBalanceRepository = userBalanceRepository;
    }

    public async Task<IEnumerable<Currency>> GetCurrenciesAsync()
    {
        return await _currencyRepository.GetActiveCurrenciesAsync();
    }

    public async Task<Currency?> GetCurrencyByIdAsync(int id)
    {
        var currency = await _currencyRepository.GetByIdAsync(id);
        return currency?.IsActive == true ? currency : null;
    }

    public async Task<Currency?> GetCurrencyByCodeAsync(string code)
    {
        var currency = await _currencyRepository.GetByCodeAsync(code);
        return currency?.IsActive == true ? currency : null;
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

        var isUnique = await _currencyRepository.IsCodeUniqueAsync(currency.Code);
        if (!isUnique)
        {
            return (false, "Валюта с таким кодом уже существует");
        }

        currency.LastUpdateTime = DateTime.UtcNow;
        currency.IsActive = true;

        await _currencyRepository.AddAsync(currency);
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

        await _currencyRepository.UpdateAsync(existingCurrency);
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
        var hasTransactions = await _transactionRepository.HasTransactionsWithCurrencyAsync(id);
        if (hasTransactions)
        {
            return (false, "Невозможно удалить валюту, так как с ней есть связанные транзакции");
        }

        // Проверяем наличие балансов пользователей в этой валюте
        var hasBalances = await _userBalanceRepository.HasBalancesWithCurrencyAsync(id);
        if (hasBalances)
        {
            return (false, "Невозможно удалить валюту, так как у пользователей есть балансы в этой валюте");
        }

        await _currencyRepository.DeleteAsync(currency);
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

        await _currencyRepository.UpdateAsync(currency);
        return (true, null);
    }
} 