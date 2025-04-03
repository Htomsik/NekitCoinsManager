using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NekitCoinsManager.Core.Data;
using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Core.Repositories;

public class CurrencyRepository : BaseRepository<Currency>, ICurrencyRepository
{
    public CurrencyRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Currency?> GetByCodeAsync(string code)
    {
        return await DbSet
            .FirstOrDefaultAsync(c => c.Code.Equals(code));
    }

    public async Task<IEnumerable<Currency>> GetActiveCurrenciesAsync()
    {
        return await DbSet
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Currency>> GetWelcomeBonusCurrenciesAsync()
    {
        return await DbSet
            .Where(c => c.IsActive && c.IsDefaultForNewUsers)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<bool> IsCodeUniqueAsync(string code, int? excludeId = null)
    {
        return !await DbSet
            .AnyAsync(c => c.Code.Equals(code) && (!excludeId.HasValue || c.Id != excludeId.Value));
    }

    public override async Task<(bool isValid, ErrorCode? error)> ValidateEntityAsync(Currency currency)
    {
        // Валидация обязательных полей
        if (string.IsNullOrWhiteSpace(currency.Name))
            return (false, ErrorCode.CurrencyNameEmpty);

        if (string.IsNullOrWhiteSpace(currency.Code))
            return (false, ErrorCode.CurrencyCodeEmpty);

        if (string.IsNullOrWhiteSpace(currency.Symbol))
            return (false, ErrorCode.CurrencySymbolEmpty);

        return (true, null);
    }

    public override async Task<(bool isValid, ErrorCode? error)> ValidateCreateAsync(Currency currency)
    {
        // Сначала выполняем базовую валидацию
        var baseValidation = await ValidateEntityAsync(currency);
        if (!baseValidation.isValid)
            return baseValidation;

        // Проверка уникальности кода
        var isUnique = await IsCodeUniqueAsync(currency.Code, currency.Id);
        if (!isUnique)
            return (false, ErrorCode.CurrencyCodeNotUnique);

        return (true, null);
    }

    public override async Task<(bool isValid, ErrorCode? error)> ValidateUpdateAsync(Currency currency)
    {
        // Проверка существования валюты
        var existingCurrency = await GetByIdAsync(currency.Id);
        if (existingCurrency == null)
            return (false, ErrorCode.CurrencyNotFound);

        // Проверка активности валюты
        if (existingCurrency.IsActive == false)
            return (false, ErrorCode.CurrencyInactive);

        // Проверка полей, которые могут быть обновлены
        if (!string.IsNullOrWhiteSpace(currency.Name) && currency.Name.Length > 50)
            return (false, ErrorCode.CurrencyNameTooLong);

        if (!string.IsNullOrWhiteSpace(currency.Symbol) && currency.Symbol.Length > 10)
            return (false, ErrorCode.CurrencySymbolTooLong);

        return (true, null);
    }

    public async Task<(bool isValid, ErrorCode? error)> ValidateExchangeRateAsync(decimal rate)
    {
        if (rate <= 0)
            return (false, ErrorCode.CurrencyRateMustBePositive);

        if (rate > 1000000) // Разумное ограничение для предотвращения ошибок
            return (false, ErrorCode.CurrencyRateTooHigh);

        return (true, null);
    }

    public override async Task<(bool canDelete, ErrorCode? error)> ValidateDeleteAsync(int id)
    {
        // Проверка существования валюты из базового метода
        var baseValidation = await base.ValidateDeleteAsync(id);
        if (!baseValidation.canDelete)
            return baseValidation;

        return (true, null);
    }

    public async Task<(bool canDelete, ErrorCode? error)> CanDeleteAsync(int currencyId, 
        ITransactionRepository transactionRepository, 
        IUserBalanceRepository userBalanceRepository)
    {
        // Проверка существования валюты
        var baseValidation = await ValidateDeleteAsync(currencyId);
        if (!baseValidation.canDelete)
            return baseValidation;

        // Проверка наличия связанных транзакций
        var hasTransactions = await transactionRepository.HasTransactionsWithCurrencyAsync(currencyId);
        if (hasTransactions)
            return (false, ErrorCode.CurrencyHasTransactions);

        // Проверка наличия связанных балансов пользователей
        var hasBalances = await userBalanceRepository.HasBalancesWithCurrencyAsync(currencyId);
        if (hasBalances)
            return (false, ErrorCode.CurrencyHasBalances);

        return (true, null);
    }
} 