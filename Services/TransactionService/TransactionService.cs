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
    private readonly IUserService _userService;
    private readonly List<ITransactionObserver> _observers = new();
    private List<Transaction> _transactions = new();

    public TransactionService(AppDbContext dbContext, IUserService userService)
    {
        _dbContext = dbContext;
        _userService = userService;
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

    public async Task<(bool success, string? error)> TransferCoinsAsync(Transaction transaction)
    {
        if (transaction.FromUser == null || transaction.ToUser == null)
        {
            return (false, "Выберите отправителя и получателя");
        }

        // Заполняем ID пользователей
        transaction.FromUserId = transaction.FromUser.Id;
        transaction.ToUserId = transaction.ToUser.Id;

        if (transaction.Amount <= 0)
        {
            return (false, "Сумма перевода должна быть больше 0");
        }

        var fromUser = await _dbContext.Users.FindAsync(transaction.FromUserId);
        var toUser = await _dbContext.Users.FindAsync(transaction.ToUserId);

        if (fromUser == null || toUser == null)
        {
            return (false, "Пользователь не найден");
        }

        if (fromUser.Id == toUser.Id)
        {
            return (false, "Нельзя переводить монеты самому себе");
        }

        if (fromUser.Balance < transaction.Amount)
        {
            return (false, "Недостаточно монет для перевода");
        }

        transaction.CreatedAt = DateTime.UtcNow;

        // Обновляем балансы через UserService
        await _userService.UpdateUserBalance(fromUser.Id, fromUser.Balance - transaction.Amount);
        await _userService.UpdateUserBalance(toUser.Id, toUser.Balance + transaction.Amount);

        _dbContext.Transactions.Add(transaction);
        await _dbContext.SaveChangesAsync();

        // Обновляем кэш транзакций
        LoadTransactions();
        NotifyObservers();
        return (true, null);
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