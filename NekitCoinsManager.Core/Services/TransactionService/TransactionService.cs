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
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<(bool success, string? error)> TransferCoinsAsync(Transaction transaction)
    {
        // Проверяем и загружаем данные пользователей, если их нет
        if (transaction.FromUser == null)
        {
            var fromUser = await _dbContext.Users.FindAsync(transaction.FromUserId);
            if (fromUser == null)
            {
                return (false, $"Отправитель с ID {transaction.FromUserId} не найден");
            }
            transaction.FromUser = fromUser;
        }
        else
        {
            transaction.FromUserId = transaction.FromUser.Id;
        }

        if (transaction.ToUser == null)
        {
            var toUser = await _dbContext.Users.FindAsync(transaction.ToUserId);
            if (toUser == null)
            {
                return (false, $"Получатель с ID {transaction.ToUserId} не найден");
            }
            transaction.ToUser = toUser;
        }
        else
        {
            transaction.ToUserId = transaction.ToUser.Id;
        }

        if (transaction.Currency == null)
        {
            var currency = await _dbContext.Currencies.FindAsync(transaction.CurrencyId);
            if (currency == null)
            {
                return (false, $"Валюта с ID {transaction.CurrencyId} не найдена");
            }
            transaction.Currency = currency;
        }
        else
        {
            transaction.CurrencyId = transaction.Currency.Id;
        }

        if (transaction.Amount <= 0)
        {
            return (false, "Сумма перевода должна быть больше 0");
        }

        if (transaction.FromUserId == transaction.ToUserId)
        {
            return (false, "Нельзя переводить монеты самому себе");
        }

        // Проверяем баланс отправителя
        var fromBalance = await _userBalanceService.GetUserBalanceAsync(transaction.FromUserId, transaction.CurrencyId);
        if (fromBalance == null || fromBalance.Amount < transaction.Amount)
        {
            return (false, "Недостаточно монет для перевода");
        }

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
        var bankAccount = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.IsBankAccount);

        if (bankAccount == null)
        {
            return (false, "Ошибка: банковский аккаунт не найден");
        }
        
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
            return (false, "Не найдено ни одной активной валюты для начисления бонуса");
        }
        
        // Выполняем транзакции для каждой валюты
        foreach (var currency in defaultCurrencies)
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
                CreatedAt = DateTime.UtcNow
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