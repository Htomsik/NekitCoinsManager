using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NekitCoinsManager.Core.Data;
using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Core.Services;

public class UserBalanceService : IUserBalanceService
{
    private readonly AppDbContext _dbContext;

    public UserBalanceService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<UserBalance>> GetUserBalancesAsync(int userId)
    {
        return await _dbContext.UserBalances
            .Include(ub => ub.Currency)
            .Where(ub => ub.UserId == userId)
            .ToListAsync();
    }

    public async Task<UserBalance?> GetUserBalanceAsync(int userId, int currencyId)
    {
        return await _dbContext.UserBalances
            .Include(ub => ub.Currency)
            .FirstOrDefaultAsync(ub => ub.UserId == userId && ub.CurrencyId == currencyId);
    }

    public async Task<(bool success, string? error)> UpdateBalanceAsync(int userId, int currencyId, decimal amount)
    {
        var balance = await GetUserBalanceAsync(userId, currencyId);
        if (balance == null)
        {
            return await CreateBalanceAsync(userId, currencyId, amount);
        }

        balance.Amount = amount;
        balance.LastUpdateTime = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
        return (true, null);
    }

    public async Task<(bool success, string? error)> CreateBalanceAsync(int userId, int currencyId, decimal amount)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null)
        {
            return (false, "Пользователь не найден");
        }

        var currency = await _dbContext.Currencies.FindAsync(currencyId);
        if (currency == null)
        {
            return (false, "Валюта не найдена");
        }

        var existingBalance = await GetUserBalanceAsync(userId, currencyId);
        if (existingBalance != null)
        {
            return (false, "Баланс для данной валюты уже существует");
        }

        var newBalance = new UserBalance
        {
            UserId = userId,
            CurrencyId = currencyId,
            Amount = amount,
            LastUpdateTime = DateTime.UtcNow
        };

        _dbContext.UserBalances.Add(newBalance);
        await _dbContext.SaveChangesAsync();
        return (true, null);
    }

    /// <summary>
    /// Проверяет возможность перевода средств
    /// </summary>
    /// <param name="fromUserId">ID отправителя</param>
    /// <param name="currencyId">ID валюты</param>
    /// <param name="amount">Сумма перевода</param>
    /// <returns>Результат проверки и сообщение об ошибке</returns>
    private async Task<(bool canTransfer, UserBalance? fromBalance, string? error)> ValidateTransferAsync(
        int fromUserId, int currencyId, decimal amount)
    {
        if (amount <= 0)
        {
            return (false, null, "Сумма перевода должна быть больше нуля");
        }

        var fromBalance = await GetUserBalanceAsync(fromUserId, currencyId);
        if (fromBalance == null)
        {
            return (false, null, "У отправителя нет баланса в данной валюте");
        }

        if (fromBalance.Amount < amount)
        {
            return (false, null, "Недостаточно средств для перевода");
        }

        return (true, fromBalance, null);
    }

    /// <summary>
    /// Получает или создает баланс получателя
    /// </summary>
    /// <param name="toUserId">ID получателя</param>
    /// <param name="currencyId">ID валюты</param>
    /// <returns>Баланс получателя и сообщение об ошибке</returns>
    private async Task<(UserBalance? toBalance, string? error)> GetOrCreateRecipientBalanceAsync(
        int toUserId, int currencyId)
    {
        var toBalance = await GetUserBalanceAsync(toUserId, currencyId);
        if (toBalance != null)
        {
            return (toBalance, null);
        }
        
        var result = await CreateBalanceAsync(toUserId, currencyId, 0);
        if (!result.success)
        {
            return (null, result.error);
        }
        
        toBalance = await GetUserBalanceAsync(toUserId, currencyId);
        return (toBalance, null);
    }

    public async Task<(bool success, string? error)> TransferBalanceAsync(int fromUserId, int toUserId, int currencyId, decimal amount)
    {
        // Валидируем возможность перевода
        var (canTransfer, fromBalance, validationError) = await ValidateTransferAsync(fromUserId, currencyId, amount);
        if (!canTransfer)
        {
            return (false, validationError);
        }
        
        // Получаем или создаем баланс получателя
        var (toBalance, balanceError) = await GetOrCreateRecipientBalanceAsync(toUserId, currencyId);
        if (toBalance == null)
        {
            return (false, balanceError);
        }

        // Выполняем перевод
        fromBalance!.Amount -= amount;
        toBalance.Amount += amount;

        // Обновляем время последнего обновления
        var now = DateTime.UtcNow;
        fromBalance.LastUpdateTime = now;
        toBalance.LastUpdateTime = now;

        await _dbContext.SaveChangesAsync();
        return (true, null);
    }

    public async Task<(bool success, string? error)> EnsureUserHasBalanceAsync(int userId, int currencyId)
    {
        var balance = await GetUserBalanceAsync(userId, currencyId);
        if (balance != null)
        {
            return (true, null);
        }

        return await CreateBalanceAsync(userId, currencyId, 0);
    }

    /// <summary>
    /// Получает баланс пользователя, а если его нет - создает с указанной начальной суммой
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <param name="currencyId">ID валюты</param>
    /// <param name="initialAmount">Начальная сумма при создании (по умолчанию 0)</param>
    /// <returns>Результат операции, сообщение об ошибке и баланс пользователя</returns>
    public async Task<(bool success, string? error, UserBalance? balance)> GetOrCreateBalanceAsync(
        int userId, int currencyId, decimal initialAmount = 0)
    {
        // Сначала пытаемся получить существующий баланс
        var balance = await GetUserBalanceAsync(userId, currencyId);
        
        // Если баланс уже существует, просто возвращаем его
        if (balance != null)
        {
            return (true, null, balance);
        }
        
        // Если баланса нет, создаем новый
        var result = await CreateBalanceAsync(userId, currencyId, initialAmount);
        if (!result.success)
        {
            return (false, result.error, null);
        }
        
        // Получаем созданный баланс
        balance = await GetUserBalanceAsync(userId, currencyId);
        
        return (true, null, balance);
    }
} 