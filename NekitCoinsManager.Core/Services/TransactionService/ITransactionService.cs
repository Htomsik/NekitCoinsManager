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
    
    void Subscribe(ITransactionObserver observer);
} 