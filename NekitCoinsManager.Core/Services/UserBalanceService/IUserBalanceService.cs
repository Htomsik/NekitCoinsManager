using System.Collections.Generic;
using System.Threading.Tasks;
using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Core.Services;

public interface IUserBalanceService
{
    Task<IEnumerable<UserBalance>> GetUserBalancesAsync(int userId);
    Task<UserBalance?> GetUserBalanceAsync(int userId, int currencyId);
    Task<(bool success, string? error)> UpdateBalanceAsync(int userId, int currencyId, decimal amount);
    Task<(bool success, string? error)> TransferBalanceAsync(int fromUserId, int toUserId, int currencyId, decimal amount);
    Task<(bool success, string? error)> CreateBalanceAsync(int userId, int currencyId, decimal amount);
    Task<(bool success, string? error, UserBalance? balance)> GetOrCreateBalanceAsync(int userId, int currencyId, decimal initialAmount = 0);
    Task<(bool isValid, string? errorMessage)> ValidateUserBalanceAsync(int userId, int currencyId, decimal amount);
    
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
    Task<(bool success, string? error)> TransferAmountBetweenBalancesAsync(
        int fromUserId,
        int fromCurrencyId,
        int toUserId,
        int toCurrencyId,
        decimal amount, 
        decimal? amountToAdd = null);
} 