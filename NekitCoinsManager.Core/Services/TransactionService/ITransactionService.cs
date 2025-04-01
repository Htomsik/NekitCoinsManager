using System.Collections.Generic;
using System.Threading.Tasks;
using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Core.Services;

public interface ITransactionObserver
{
    void OnTransactionsChanged();
}

public interface ITransactionService
{
    Task<IEnumerable<Transaction>> GetTransactionsAsync();
    Task<(bool success, string? error)> TransferCoinsAsync(Transaction transaction);
    Task<(bool success, string? error)> GrantWelcomeBonusAsync(int userId);
    
    /// <summary>
    /// Пополняет баланс пользователя (депозит)
    /// </summary>
    /// <param name="transaction">Транзакция для пополнения баланса</param>
    /// <returns>Результат операции и сообщение об ошибке</returns>
    Task<(bool success, string? error)> DepositCoinsAsync(Transaction transaction);
    
    /// <summary>
    /// Конвертирует средства пользователя из одной валюты в другую
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <param name="fromCurrencyId">ID исходной валюты</param>
    /// <param name="toCurrencyId">ID целевой валюты</param>
    /// <param name="amount">Сумма для конвертации в исходной валюте</param>
    /// <returns>Результат операции, сообщение об ошибке и сконвертированная сумма</returns>
    Task<(bool success, string? error, decimal? convertedAmount)> ConvertCurrencyAsync(
        int userId, 
        int fromCurrencyId, 
        int toCurrencyId, 
        decimal amount);
    
    void Subscribe(ITransactionObserver observer);
} 