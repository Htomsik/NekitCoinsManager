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
    private readonly List<ITransactionObserver> _observers = new();

    public TransactionService(AppDbContext dbContext, IUserBalanceService userBalanceService)
    {
        _dbContext = dbContext;
        _userBalanceService = userBalanceService;
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