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
    void Subscribe(ITransactionObserver observer);
} 