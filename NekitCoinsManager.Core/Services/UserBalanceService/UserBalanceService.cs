using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NekitCoinsManager.Core.Data;
using NekitCoinsManager.Core.Models;
using NekitCoinsManager.Core.Repositories;

namespace NekitCoinsManager.Core.Services;

public class UserBalanceService : IUserBalanceService
{
    private readonly IUserBalanceRepository _userBalanceRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrencyRepository _currencyRepository;

    public UserBalanceService(
        IUserBalanceRepository userBalanceRepository,
        IUserRepository userRepository,
        ICurrencyRepository currencyRepository)
    {
        _userBalanceRepository = userBalanceRepository;
        _userRepository = userRepository;
        _currencyRepository = currencyRepository;
    }

    public async Task<IEnumerable<UserBalance>> GetUserBalancesAsync(int userId)
    {
        return await _userBalanceRepository.GetUserBalancesAsync(userId);
    }

    public async Task<UserBalance?> GetUserBalanceAsync(int userId, int currencyId)
    {
        return await _userBalanceRepository.GetUserBalanceAsync(userId, currencyId);
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

        await _userBalanceRepository.UpdateAsync(balance);
        return (true, null);
    }

    public async Task<(bool success, string? error)> CreateBalanceAsync(int userId, int currencyId, decimal amount)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return (false, "Пользователь не найден");
        }

        var currency = await _currencyRepository.GetByIdAsync(currencyId);
        if (currency == null)
        {
            return (false, "Валюта не найдена");
        }

        // Проверяем, существует ли уже баланс для этой валюты у пользователя
        var existingBalance = await _userBalanceRepository.GetUserBalanceAsync(userId, currencyId);
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

        await _userBalanceRepository.AddAsync(newBalance);
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

        var fromBalance = await _userBalanceRepository.GetUserBalanceAsync(fromUserId, currencyId);
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
        var toBalance = await _userBalanceRepository.GetUserBalanceAsync(toUserId, currencyId);
        if (toBalance != null)
        {
            return (toBalance, null);
        }
        
        var result = await CreateBalanceAsync(toUserId, currencyId, 0);
        if (!result.success)
        {
            return (null, result.error);
        }
        
        toBalance = await _userBalanceRepository.GetUserBalanceAsync(toUserId, currencyId);
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

        // Обновляем оба баланса
        await _userBalanceRepository.UpdateAsync(fromBalance);
        await _userBalanceRepository.UpdateAsync(toBalance);
        
        return (true, null);
    }

    public async Task<(bool success, string? error)> EnsureUserHasBalanceAsync(int userId, int currencyId)
    {
        var hasBalance = await _userBalanceRepository.HasBalanceAsync(userId, currencyId);
        if (hasBalance)
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
        var balance = await _userBalanceRepository.GetUserBalanceAsync(userId, currencyId);
        
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
        balance = await _userBalanceRepository.GetUserBalanceAsync(userId, currencyId);
        
        return (true, null, balance);
    }
} 