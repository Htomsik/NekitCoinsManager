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
        // Выполняем валидацию на уровне репозитория
        var (isValid, validationError) = await _currencyRepository.ValidateCreateAsync(currency);
        if (!isValid)
        {
            // Преобразуем технические коды ошибок в понятные пользователю сообщения
            string userError = validationError switch
            {
                ErrorCode.CurrencyNameEmpty => "Название валюты не может быть пустым",
                ErrorCode.CurrencyCodeEmpty => "Код валюты не может быть пустым",
                ErrorCode.CurrencySymbolEmpty => "Символ валюты не может быть пустым",
                ErrorCode.CurrencyCodeNotUnique => "Валюта с таким кодом уже существует",
                _ => "Ошибка валидации валюты"
            };
            return (false, userError);
        }

        // Применяем бизнес-логику
        currency.LastUpdateTime = DateTime.UtcNow;
        currency.IsActive = true;

        // Сохраняем валюту
        await _currencyRepository.AddAsync(currency);
        return (true, null);
    }

    public async Task<(bool success, string? error)> UpdateCurrencyAsync(Currency currency)
    {
        var (isValid, validationError) = await _currencyRepository.ValidateUpdateAsync(currency);
        if (!isValid)
        {
            // Преобразуем технические коды ошибок в понятные пользователю сообщения
            string userError = validationError switch
            {
                ErrorCode.CurrencyNotFound => "Валюта не найдена",
                ErrorCode.CurrencyInactive => "Валюта неактивна",
                ErrorCode.CurrencyNameTooLong => "Название валюты слишком длинное",
                ErrorCode.CurrencySymbolTooLong => "Символ валюты слишком длинный",
                _ => "Ошибка валидации валюты"
            };
            return (false, userError);
        }

        var existingCurrency = await _currencyRepository.GetByIdAsync(currency.Id);

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
        var (canDelete, deleteError) = await _currencyRepository.CanDeleteAsync(id, _transactionRepository, _userBalanceRepository);
        if (!canDelete)
        {
            // Преобразуем технические коды ошибок в понятные пользователю сообщения
            string userError = deleteError switch
            {
                ErrorCode.CommonEntityNotFound => "Валюта не найдена",
                ErrorCode.CurrencyNotFound => "Валюта не найдена",
                ErrorCode.CurrencyHasTransactions => "Невозможно удалить валюту, так как с ней есть связанные транзакции",
                ErrorCode.CurrencyHasBalances => "Невозможно удалить валюту, так как у пользователей есть балансы в этой валюте",
                _ => "Ошибка при удалении валюты"
            };
            return (false, userError);
        }

        var currency = await _currencyRepository.GetByIdAsync(id);
        await _currencyRepository.DeleteAsync(currency);
        return (true, null);
    }

    public async Task<(bool success, string? error)> UpdateExchangeRateAsync(int currencyId, decimal newRate)
    {
        // Валидация курса обмена
        var (isRateValid, rateError) = await _currencyRepository.ValidateExchangeRateAsync(newRate);
        if (!isRateValid)
        {
            string userError = rateError switch
            {
                ErrorCode.CurrencyRateMustBePositive => "Курс обмена должен быть больше нуля",
                ErrorCode.CurrencyRateTooHigh => "Указан слишком высокий курс обмена",
                _ => "Ошибка валидации курса обмена"
            };
            return (false, userError);
        }

        // Проверка существования валюты
        var currency = await _currencyRepository.GetByIdAsync(currencyId);
        if (currency == null)
        {
            return (false, "Валюта не найдена");
        }

        currency.ExchangeRate = newRate;
        currency.LastUpdateTime = DateTime.UtcNow;

        await _currencyRepository.UpdateAsync(currency);
        return (true, null);
    }
} 