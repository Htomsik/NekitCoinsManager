using System.Collections.Generic;
using System.Threading.Tasks;
using NekitCoinsManager.Models;

namespace NekitCoinsManager.Services;

public interface ITransactionObserver
{
    void OnTransactionsChanged();
}

public interface ITransactionService
{
    IEnumerable<Transaction> GetTransactions();
    Task<(bool success, string? error)> TransferCoinsAsync(Transaction transaction);
    void Subscribe(ITransactionObserver observer);
} 