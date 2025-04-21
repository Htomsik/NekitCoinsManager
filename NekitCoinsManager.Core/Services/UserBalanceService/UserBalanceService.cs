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

        // Валидируем обновление баланса
        var (isValid, validationError) = await _userBalanceRepository.ValidateUpdateAsync(balance);
        if (!isValid)
        {
            // Преобразуем технические коды ошибок в понятные пользователю сообщения
            string userError = validationError switch
            {
                ErrorCode.BalanceNotFound => "Баланс не найден",
                ErrorCode.BalanceAmountNegative => "Сумма не может быть отрицательной",
                _ => "Ошибка при обновлении баланса"
            };
            return (false, userError);
        }

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

        var newBalance = new UserBalance
        {
            UserId = userId,
            CurrencyId = currencyId,
            Amount = amount,
            LastUpdateTime = DateTime.UtcNow
        };

        // Используем новый метод валидации при создании
        var (isValid, validationError) = await _userBalanceRepository.ValidateCreateAsync(newBalance);
        if (!isValid)
        {
            // Преобразуем технические коды ошибок в понятные пользователю сообщения
            string userError = validationError switch
            {
                ErrorCode.BalanceUserIdInvalid => "Неверный идентификатор пользователя",
                ErrorCode.TransactionCurrencyIdInvalid => "Неверный идентификатор валюты",
                ErrorCode.BalanceAmountNegative => "Сумма не может быть отрицательной",
                ErrorCode.BalanceAlreadyExists => "Баланс для данной валюты уже существует",
                _ => "Ошибка при создании баланса"
            };
            return (false, userError);
        }

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
        // Используем новый метод валидации операций с балансом
        var (isValid, validationError) = await _userBalanceRepository.ValidateBalanceOperationAsync(
            fromUserId, currencyId, amount, true);

        if (!isValid)
        {
            // Преобразуем технические коды ошибок в понятные пользователю сообщения
            string userError = validationError switch
            {
                ErrorCode.TransactionAmountMustBePositive => "Сумма перевода должна быть больше нуля",
                ErrorCode.BalanceNotFound => "У отправителя нет баланса в данной валюте",
                ErrorCode.TransactionInsufficientFunds => "Недостаточно средств для перевода",
                _ => "Ошибка при проверке возможности перевода"
            };
            return (false, null, userError);
        }

        // Получаем баланс отправителя для дальнейших операций
        var fromBalance = await _userBalanceRepository.GetUserBalanceAsync(fromUserId, currencyId);
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

    /// <summary>
    /// Проверяет, достаточно ли у пользователя средств для операции
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <param name="currencyId">ID валюты</param>
    /// <param name="amount">Сумма операции</param>
    /// <returns>Результат проверки и сообщение об ошибке</returns>
    public async Task<(bool isValid, string? errorMessage)> ValidateUserBalanceAsync(int userId, int currencyId, decimal amount)
    {
        // Проверяем баланс пользователя через репозиторий балансов
        var (isValid, error) = await _userBalanceRepository.ValidateBalanceOperationAsync(userId, currencyId, amount);
        
        if (!isValid)
        {
            // Преобразуем технический код ошибки в понятное пользователю сообщение
            string userError = error switch
            {
                ErrorCode.TransactionInsufficientFunds => "Недостаточно средств на счете",
                ErrorCode.BalanceNotFound => "У пользователя нет баланса в указанной валюте",
                _ => "Ошибка при проверке баланса пользователя"
            };
            
            return (false, userError);
        }
        
        return (true, null);
    }

    /// <summary>
    /// Переводит указанную сумму с одного баланса на другой
    /// </summary>
    /// <param name="fromUserId">Идентификатор пользователя-отправителя</param>
    /// <param name="fromCurrencyId">Идентификатор валюты списания</param>
    /// <param name="toUserId">Идентификатор пользователя-получателя</param>
    /// <param name="toCurrencyId">Идентификатор валюты зачисления</param>
    /// <param name="amount">Сумма для списания</param>
    /// <param name="amountToAdd">Сумма для зачисления (если отличается от amount)</param>
    /// <returns>Результат операции и сообщение об ошибке</returns>
    public async Task<(bool success, string? error)> TransferAmountBetweenBalancesAsync(
        int fromUserId,
        int fromCurrencyId,
        int toUserId,
        int toCurrencyId,
        decimal amount,
        decimal? amountToAdd = null)
    {
        // Получаем баланс отправителя
        var (fromBalanceSuccess, fromBalanceError, fromBalance) = await GetOrCreateBalanceAsync(fromUserId, fromCurrencyId);
        if (!fromBalanceSuccess || fromBalance == null)
        {
            return (false, fromBalanceError ?? $"Не удалось получить баланс пользователя с ID {fromUserId} в валюте с ID {fromCurrencyId}");
        }
        
        // Получаем баланс получателя
        var (toBalanceSuccess, toBalanceError, toBalance) = await GetOrCreateBalanceAsync(toUserId, toCurrencyId);
        if (!toBalanceSuccess || toBalance == null)
        {
            return (false, toBalanceError ?? $"Не удалось получить баланс пользователя с ID {toUserId} в валюте с ID {toCurrencyId}");
        }
        
        // Проверяем, является ли отправитель банком и достаточно ли у него средств
        var fromUser = await _userRepository.GetByIdAsync(fromUserId);
        if (fromUser?.IsBankAccount == true && fromBalance.Amount < amount)
        {
            return (false, $"Банк не имеет достаточно средств в валюте для выполнения транзакции");
        }
        
        // Проверяем, что у отправителя достаточно средств
        if (fromBalance.Amount < amount)
        {
            return (false, "Недостаточно средств для перевода");
        }
        
        // Определяем сумму для зачисления
        decimal actualAmountToAdd = amountToAdd ?? amount;
        
        // Обновляем балансы
        fromBalance.Amount -= amount;
        toBalance.Amount += actualAmountToAdd;
        
        // Устанавливаем время последнего обновления
        var currentTime = DateTime.UtcNow;
        fromBalance.LastUpdateTime = currentTime;
        toBalance.LastUpdateTime = currentTime;
        
        // Сохраняем изменения
        await _userBalanceRepository.UpdateAsync(fromBalance);
        await _userBalanceRepository.UpdateAsync(toBalance);
        
        return (true, null);
    }
} 