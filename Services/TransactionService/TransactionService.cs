using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NekitCoinsManager.Data;
using NekitCoinsManager.Models;

namespace NekitCoinsManager.Services;

public class TransactionService : ITransactionService
{
    private readonly AppDbContext _dbContext;
    private readonly List<ITransactionObserver> _observers = new();
    private List<Transaction> _transactions = new();

    public TransactionService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
        LoadTransactions();
    }

    private void LoadTransactions()
    {
        _transactions = _dbContext.Transactions
            .Include(t => t.FromUser)
            .Include(t => t.ToUser)
            .OrderByDescending(t => t.CreatedAt)
            .ToList();
    }

    public IEnumerable<Transaction> GetTransactions()
    {
        return _transactions;
    }

    public async Task TransferCoins(Transaction transaction)
    {
        if (transaction.Amount <= 0)
        {
            throw new Exception("Сумма перевода должна быть больше 0");
        }

        var fromUser = await _dbContext.Users.FindAsync(transaction.FromUserId);
        var toUser = await _dbContext.Users.FindAsync(transaction.ToUserId);

        if (fromUser == null || toUser == null)
        {
            throw new Exception("Пользователь не найден");
        }

        if (fromUser.Id == toUser.Id)
        {
            throw new Exception("Нельзя переводить монеты самому себе");
        }

        if (fromUser.Balance < transaction.Amount)
        {
            throw new Exception("Недостаточно монет для перевода");
        }

        transaction.CreatedAt = DateTime.UtcNow;

        fromUser.Balance -= transaction.Amount;
        toUser.Balance += transaction.Amount;

        _dbContext.Transactions.Add(transaction);
        await _dbContext.SaveChangesAsync();

        // Обновляем кэш транзакций
        LoadTransactions();
        NotifyObservers();
    }

    public void Subscribe(ITransactionObserver observer)
    {
        if (!_observers.Contains(observer))
        {
            _observers.Add(observer);
        }
    }

    private void NotifyObservers()
    {
        foreach (var observer in _observers)
        {
            observer.OnTransactionsChanged();
        }
    }
} 