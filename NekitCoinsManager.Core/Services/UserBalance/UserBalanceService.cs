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

    public async Task<(bool success, string? error)> TransferBalanceAsync(int fromUserId, int toUserId, int currencyId, decimal amount)
    {
        if (amount <= 0)
        {
            return (false, "Сумма перевода должна быть больше нуля");
        }

        var fromBalance = await GetUserBalanceAsync(fromUserId, currencyId);
        if (fromBalance == null)
        {
            return (false, "У отправителя нет баланса в данной валюте");
        }

        if (fromBalance.Amount < amount)
        {
            return (false, "Недостаточно средств для перевода");
        }

        var toBalance = await GetUserBalanceAsync(toUserId, currencyId);
        if (toBalance == null)
        {
            var result = await CreateBalanceAsync(toUserId, currencyId, 0);
            if (!result.success)
            {
                return result;
            }
            toBalance = await GetUserBalanceAsync(toUserId, currencyId);
        }

        fromBalance.Amount -= amount;
        toBalance!.Amount += amount;

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
} 