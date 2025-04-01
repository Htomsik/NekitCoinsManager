using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NekitCoinsManager.Core.Data;
using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Core.Services;

public class TransactionService : ITransactionService
{
    private readonly AppDbContext _dbContext;
    private readonly IUserBalanceService _userBalanceService;
    private readonly ICurrencyConversionService _currencyConversionService;
    private readonly List<ITransactionObserver> _observers = new();

    public TransactionService(
        AppDbContext dbContext, 
        IUserBalanceService userBalanceService,
        ICurrencyConversionService currencyConversionService)
    {
        _dbContext = dbContext;
        _userBalanceService = userBalanceService;
        _currencyConversionService = currencyConversionService;
    }

    public async Task<IEnumerable<Transaction>> GetTransactionsAsync()
    {
        return await _dbContext.Transactions
            .Include(t => t.FromUser)
            .Include(t => t.ToUser)
            .Include(t => t.Currency)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Проверяет валидность транзакции
    /// </summary>
    /// <param name="transaction">Транзакция для проверки</param>
    /// <returns>Результат проверки и сообщение об ошибке</returns>
    private async Task<(bool isValid, string? errorMessage)> ValidateTransactionAsync(Transaction transaction)
    {
        // Проверяем валидность ID пользователей и валюты
        if (transaction.FromUserId <= 0)
        {
            return (false, "Не указан отправитель перевода");
        }

        if (transaction.ToUserId <= 0)
        {
            return (false, "Не указан получатель перевода");
        }

        if (transaction.CurrencyId <= 0)
        {
            return (false, "Не указана валюта перевода");
        }

        // Проверяем существование пользователей и валюты в базе данных
        var fromUserExists = await _dbContext.Users.AnyAsync(u => u.Id == transaction.FromUserId);
        if (!fromUserExists)
        {
            return (false, $"Отправитель с ID {transaction.FromUserId} не найден");
        }

        var toUserExists = await _dbContext.Users.AnyAsync(u => u.Id == transaction.ToUserId);
        if (!toUserExists)
        {
            return (false, $"Получатель с ID {transaction.ToUserId} не найден");
        }

        var currencyExists = await _dbContext.Currencies.AnyAsync(c => c.Id == transaction.CurrencyId);
        if (!currencyExists)
        {
            return (false, $"Валюта с ID {transaction.CurrencyId} не найдена");
        }

        if (transaction.FromUserId == transaction.ToUserId)
        {
            return (false, "Нельзя переводить монеты самому себе");
        }

        // Проверка суммы перевода и баланса выполняется в UserBalanceService,
        // поэтому здесь эти проверки не дублируем

        return (true, null);
    }

    /// <summary>
    /// Проверяет валидность депозита
    /// </summary>
    /// <param name="transaction">Транзакция депозита</param>
    /// <returns>Результат проверки и сообщение об ошибке</returns>
    private async Task<(bool isValid, string? errorMessage)> ValidateDepositAsync(Transaction transaction)
    {
        if (transaction.ToUserId <= 0)
        {
            return (false, "Не указан пользователь для пополнения");
        }

        if (transaction.CurrencyId <= 0)
        {
            return (false, "Не указана валюта для пополнения");
        }

        if (transaction.Amount <= 0)
        {
            return (false, "Сумма пополнения должна быть больше нуля");
        }

        // Проверяем существование пользователя и валюты
        var userExists = await _dbContext.Users.AnyAsync(u => u.Id == transaction.ToUserId);
        if (!userExists)
        {
            return (false, $"Пользователь с ID {transaction.ToUserId} не найден");
        }

        var currencyExists = await _dbContext.Currencies.AnyAsync(c => c.Id == transaction.CurrencyId);
        if (!currencyExists)
        {
            return (false, $"Валюта с ID {transaction.CurrencyId} не найдена");
        }

        return (true, null);
    }

    public async Task<(bool success, string? error)> TransferCoinsAsync(Transaction transaction)
    {
        // Валидируем транзакцию
        var (isValid, errorMessage) = await ValidateTransactionAsync(transaction);
        if (!isValid)
        {
            return (false, errorMessage);
        }

        // Устанавливаем время создания, если не задано
        if (transaction.CreatedAt == default)
        {
            transaction.CreatedAt = DateTime.UtcNow;
        }
        
        // Выполняем перевод через UserBalanceService
        var (success, error) = await _userBalanceService.TransferBalanceAsync(
            transaction.FromUserId, 
            transaction.ToUserId, 
            transaction.CurrencyId, 
            transaction.Amount
        );

        if (!success)
        {
            return (false, error ?? "Ошибка при переводе средств");
        }

        _dbContext.Transactions.Add(transaction);
        await _dbContext.SaveChangesAsync();

        NotifyObservers();
        return (true, null);
    }

    /// <summary>
    /// Пополняет баланс пользователя (депозит)
    /// </summary>
    /// <param name="transaction">Транзакция для пополнения баланса</param>
    /// <returns>Результат операции и сообщение об ошибке</returns>
    public async Task<(bool success, string? error)> DepositCoinsAsync(Transaction transaction)
    {
        // Валидируем данные депозита
        var (isValid, errorMessage) = await ValidateDepositAsync(transaction);
        if (!isValid)
        {
            return (false, errorMessage);
        }

        // Получаем банковский аккаунт для использования как источник пополнения
        var (bankAccount, bankError) = await GetBankAccountAsync();
        if (bankAccount == null)
        {
            return (false, bankError ?? "Не удалось найти банковский аккаунт для пополнения");
        }

        // Устанавливаем свойства для транзакции депозита
        transaction.FromUserId = bankAccount.Id; // Используем банковский аккаунт как отправителя
        transaction.Type = TransactionType.Deposit;
        
        // Устанавливаем время создания, если не задано
        if (transaction.CreatedAt == default)
        {
            transaction.CreatedAt = DateTime.UtcNow;
        }
        
        // Устанавливаем комментарий, если не задан
        if (string.IsNullOrEmpty(transaction.Comment))
        {
            transaction.Comment = "Пополнение баланса";
        }

        // Получаем или создаем баланс пользователя
        var balance = await _userBalanceService.GetUserBalanceAsync(transaction.ToUserId, transaction.CurrencyId);
        if (balance == null)
        {
            var createResult = await _userBalanceService.CreateBalanceAsync(transaction.ToUserId, transaction.CurrencyId, 0);
            if (!createResult.success)
            {
                return (false, createResult.error ?? "Не удалось создать баланс пользователя");
            }
            
            balance = await _userBalanceService.GetUserBalanceAsync(transaction.ToUserId, transaction.CurrencyId);
        }

        // Увеличиваем баланс пользователя
        balance!.Amount += transaction.Amount;
        balance.LastUpdateTime = DateTime.UtcNow;
        
        // Сохраняем транзакцию и обновляем баланс
        _dbContext.Transactions.Add(transaction);
        await _dbContext.SaveChangesAsync();

        NotifyObservers();
        return (true, null);
    }

    /// <summary>
    /// Получает банковский аккаунт
    /// </summary>
    /// <returns>Банковский аккаунт или null, если не найден</returns>
    private async Task<(User? user, string? error)> GetBankAccountAsync()
    {
        var bankAccount = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.IsBankAccount);

        if (bankAccount == null)
        {
            return (null, "Ошибка: банковский аккаунт не найден");
        }

        return (bankAccount, null);
    }

    /// <summary>
    /// Получает список валют для начисления приветственного бонуса
    /// </summary>
    /// <returns>Список валют или пустой список, если валюты не найдены</returns>
    private async Task<(List<Currency> currencies, string? error)> GetWelcomeBonusCurrenciesAsync()
    {
        // Получаем валюты, начисляемые при регистрации
        var defaultCurrencies = await _dbContext.Currencies
            .Where(c => c.IsActive && c.IsDefaultForNewUsers)
            .ToListAsync();
        
        if (!defaultCurrencies.Any())
        {
            // Если нет валют с признаком IsDefaultForNewUsers, используем валюту с самым низким курсом
            var lowestRateCurrency = await _dbContext.Currencies
                .Where(c => c.IsActive)
                .OrderBy(c => c.ExchangeRate)
                .FirstOrDefaultAsync();
                
            if (lowestRateCurrency != null)
            {
                defaultCurrencies = new List<Currency> { lowestRateCurrency };
            }
        }
        
        if (!defaultCurrencies.Any())
        {
            return (new List<Currency>(), "Не найдено ни одной активной валюты для начисления бонуса");
        }

        return (defaultCurrencies, null);
    }
    
    /// <summary>
    /// Начисляет приветственный бонус новому пользователю
    /// </summary>
    /// <param name="userId">ID нового пользователя</param>
    /// <returns>Результат операции и сообщение об ошибке</returns>
    public async Task<(bool success, string? error)> GrantWelcomeBonusAsync(int userId)
    {
        // Получаем пользователя
        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null)
        {
            return (false, $"Пользователь с ID {userId} не найден");
        }
        
        // Получаем банковский аккаунт
        var (bankAccount, bankError) = await GetBankAccountAsync();
        if (bankAccount == null)
        {
            return (false, bankError);
        }
        
        // Получаем валюты для начисления бонуса
        var (currencies, currencyError) = await GetWelcomeBonusCurrenciesAsync();
        if (!currencies.Any())
        {
            return (false, currencyError);
        }
        
        // Выполняем транзакции для каждой валюты
        foreach (var currency in currencies)
        {
            // Определяем количество начисляемой валюты
            decimal amount = currency.IsDefaultForNewUsers ? currency.DefaultAmount : 100;
            
            // Создаем транзакцию от банка к новому пользователю
            var transaction = new Transaction
            {
                FromUserId = bankAccount.Id,
                ToUserId = user.Id,
                CurrencyId = currency.Id,
                Amount = amount,
                Comment = $"Приветственный бонус для {user.Username}",
                CreatedAt = DateTime.UtcNow,
                Type = TransactionType.Transfer
            };
            
            // Выполняем транзакцию
            var (success, error) = await TransferCoinsAsync(transaction);
            if (!success)
            {
                return (false, error ?? $"Не удалось начислить приветственный бонус в валюте {currency.Name}");
            }
        }
        
        return (true, null);
    }

    /// <summary>
    /// Проверяет валидность конвертации валюты
    /// </summary>
    /// <param name="userId">ID пользователя, выполняющего конвертацию</param>
    /// <param name="fromCurrencyId">ID исходной валюты</param>
    /// <param name="toCurrencyId">ID целевой валюты</param>
    /// <param name="amount">Сумма для конвертации</param>
    /// <returns>Результат проверки, сообщение об ошибке, а также найденные сущности для дальнейшего использования</returns>
    private async Task<(bool isValid, string? errorMessage, User? user, Currency? fromCurrency, Currency? toCurrency, UserBalance? fromBalance)> 
        ValidateConversionAsync(int userId, int fromCurrencyId, int toCurrencyId, decimal amount)
    {
        // Проверяем, что валюты различаются
        if (fromCurrencyId == toCurrencyId)
        {
            return (false, "Нельзя конвертировать валюту саму в себя", null, null, null, null);
        }
        
        // Проверяем, что сумма положительна
        if (amount <= 0)
        {
            return (false, "Сумма для конвертации должна быть больше нуля", null, null, null, null);
        }
        
        // Получаем пользователя
        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null)
        {
            return (false, $"Пользователь с ID {userId} не найден", null, null, null, null);
        }
        
        // Получаем исходную валюту
        var fromCurrency = await _dbContext.Currencies.FindAsync(fromCurrencyId);
        if (fromCurrency == null)
        {
            return (false, $"Исходная валюта с ID {fromCurrencyId} не найдена", user, null, null, null);
        }
        
        // Получаем целевую валюту
        var toCurrency = await _dbContext.Currencies.FindAsync(toCurrencyId);
        if (toCurrency == null)
        {
            return (false, $"Целевая валюта с ID {toCurrencyId} не найдена", user, fromCurrency, null, null);
        }
        
        // Проверяем баланс пользователя в исходной валюте
        var fromBalance = await _userBalanceService.GetUserBalanceAsync(userId, fromCurrencyId);
        if (fromBalance == null || fromBalance.Amount < amount)
        {
            return (false, $"Недостаточно средств для конвертации. Требуется: {amount} {fromCurrency.Code}, доступно: {fromBalance?.Amount ?? 0} {fromCurrency.Code}", 
                   user, fromCurrency, toCurrency, null);
        }
        
        return (true, null, user, fromCurrency, toCurrency, fromBalance);
    }

    /// <summary>
    /// Конвертирует указанную сумму из одной валюты в другую
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <param name="fromCurrencyId">ID исходной валюты</param>
    /// <param name="toCurrencyId">ID целевой валюты</param>
    /// <param name="amount">Сумма для конвертации</param>
    /// <returns>Результат операции, сообщение об ошибке и сконвертированная сумма</returns>
    public async Task<(bool success, string? error, decimal? convertedAmount)> ConvertCurrencyAsync(
        int userId, 
        int fromCurrencyId, 
        int toCurrencyId, 
        decimal amount)
    {
        // Валидируем данные для конвертации
        var (isValid, errorMessage, user, fromCurrency, toCurrency, fromBalance) = 
            await ValidateConversionAsync(userId, fromCurrencyId, toCurrencyId, amount);
        
        if (!isValid)
        {
            return (false, errorMessage, null);
        }
        
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        
        try
        {
            // Конвертируем сумму с помощью сервиса конвертации
            decimal convertedAmount = await _currencyConversionService.ConvertAsync(
                amount, 
                fromCurrency!.Code, 
                toCurrency!.Code);
            
            // Получаем или создаем баланс пользователя в целевой валюте
            var (toBalanceSuccess, toBalanceError, toBalance) = await _userBalanceService.GetOrCreateBalanceAsync(userId, toCurrencyId);
            if (!toBalanceSuccess || toBalance == null)
            {
                return (false, toBalanceError ?? "Не удалось получить или создать баланс пользователя в целевой валюте", null);
            }
            
            // Получаем банковский аккаунт для записи транзакций
            var (bankAccount, bankError) = await GetBankAccountAsync();
            if (bankAccount == null)
            {
                return (false, bankError ?? "Не удалось найти банковский аккаунт для операции конвертации", null);
            }
            
            // Проверяем баланс банка в целевой валюте
            var bankBalance = await _userBalanceService.GetUserBalanceAsync(bankAccount.Id, toCurrencyId);
            if (bankBalance == null || bankBalance.Amount < convertedAmount)
            {
                return (false, $"Банк не имеет достаточно средств в валюте {toCurrency.Code} для выполнения конвертации. Требуется: {convertedAmount} {toCurrency.Code}, доступно: {bankBalance?.Amount ?? 0} {toCurrency.Code}", null);
            }
            
            // Обновляем балансы
            fromBalance!.Amount -= amount;
            fromBalance.LastUpdateTime = DateTime.UtcNow;
            
            toBalance!.Amount += convertedAmount;
            toBalance.LastUpdateTime = DateTime.UtcNow;
            
            // Обновляем баланс банка
            bankBalance!.Amount -= convertedAmount;
            bankBalance.LastUpdateTime = DateTime.UtcNow;
            
            // Создаем транзакции
            // 1. Списание исходной валюты
            var withdrawTransaction = new Transaction
            {
                FromUserId = userId,
                ToUserId = bankAccount.Id,
                CurrencyId = fromCurrencyId,
                Amount = amount,
                Comment = $"Конвертация {amount} {fromCurrency.Code} в {toCurrency.Code}",
                CreatedAt = DateTime.UtcNow,
                Type = TransactionType.Conversion
            };
            
            // Сначала добавляем транзакцию списания, чтобы получить ее ID
            _dbContext.Transactions.Add(withdrawTransaction);
            await _dbContext.SaveChangesAsync();
            
            // 2. Зачисление целевой валюты
            var depositTransaction = new Transaction
            {
                FromUserId = bankAccount.Id,
                ToUserId = userId,
                CurrencyId = toCurrencyId,
                Amount = convertedAmount,
                Comment = $"Получено в результате конвертации {amount} {fromCurrency.Code}",
                CreatedAt = DateTime.UtcNow,
                Type = TransactionType.Conversion,
                ParentTransactionId = withdrawTransaction.Id // Связь с транзакцией списания
            };
            
            // Добавляем транзакцию зачисления
            _dbContext.Transactions.Add(depositTransaction);
            
            // Применяем комиссию, если она предусмотрена (примерная логика)
            decimal feePercentage = 0.01m; // 1% комиссия
            decimal feeAmount = amount * feePercentage;
            
            if (feeAmount > 0)
            {
                // Создаем транзакцию комиссии
                var feeTransaction = new Transaction
                {
                    FromUserId = userId,
                    ToUserId = bankAccount.Id,
                    CurrencyId = fromCurrencyId,
                    Amount = feeAmount,
                    Comment = $"Комиссия за конвертацию {amount} {fromCurrency.Code}",
                    CreatedAt = DateTime.UtcNow,
                    Type = TransactionType.Fee,
                    ParentTransactionId = withdrawTransaction.Id // Связь с основной транзакцией
                };
                
                // Списываем комиссию с баланса пользователя
                fromBalance!.Amount -= feeAmount;
                
                // Добавляем транзакцию комиссии
                _dbContext.Transactions.Add(feeTransaction);
            }
            
            await _dbContext.SaveChangesAsync();
            
            // Фиксируем транзакцию в БД
            await transaction.CommitAsync();
            
            // Уведомляем наблюдателей
            NotifyObservers();
            
            return (true, null, convertedAmount);
        }
        catch (Exception ex)
        {
            // В случае ошибки откатываем изменения
            await transaction.RollbackAsync();
            return (false, $"Ошибка при конвертации валюты: {ex.Message}", null);
        }
    }

    public void Subscribe(ITransactionObserver observer)
    {
        if (!_observers.Contains(observer))
        {
            _observers.Add(observer);
        }
    }

    private void NotifyObservers()
    {
        foreach (var observer in _observers)
        {
            observer.OnTransactionsChanged();
        }
    }
} 