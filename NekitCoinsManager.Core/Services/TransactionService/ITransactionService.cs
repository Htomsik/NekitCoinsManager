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
    void Subscribe(ITransactionObserver observer);
} 