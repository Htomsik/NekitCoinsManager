using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NekitCoinsManager.Core.Data;
using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Core.Services;

public class TransactionService : ITransactionService
{
    private readonly AppDbContext _dbContext;
    private readonly IUserBalanceService _userBalanceService;
    private readonly List<ITransactionObserver> _observers = new();

    public TransactionService(AppDbContext dbContext, IUserBalanceService userBalanceService)
    {
        _dbContext = dbContext;
        _userBalanceService = userBalanceService;
    }

    public async Task<IEnumerable<Transaction>> GetTransactionsAsync()
    {
        return await _dbContext.Transactions
            .Include(t => t.FromUser)
            .Include(t => t.ToUser)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<(bool success, string? error)> TransferCoinsAsync(Transaction transaction)
    {
        if (transaction.FromUser == null || transaction.ToUser == null)
        {
            return (false, "Выберите отправителя и получателя");
        }

        // Заполняем недостающие данные
        transaction.FromUserId = transaction.FromUser.Id;
        transaction.ToUserId = transaction.ToUser.Id;
        transaction.CurrencyId = transaction.Currency.Id;

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

        // Проверяем баланс отправителя
        var fromBalance = await _userBalanceService.GetUserBalanceAsync(fromUser.Id, transaction.CurrencyId);
        if (fromBalance == null || fromBalance.Amount < transaction.Amount)
        {
            return (false, "Недостаточно монет для перевода");
        }

        transaction.CreatedAt = DateTime.UtcNow;

        // Выполняем перевод через UserBalanceService
        var (success, error) = await _userBalanceService.TransferBalanceAsync(
            fromUser.Id, 
            toUser.Id, 
            transaction.CurrencyId, 
            transaction.Amount
        );

        if (!success)
        {
            return (false, error ?? "Ошибка при переводе средств");
        }

        _dbContext.Transactions.Add(transaction);
        await _dbContext.SaveChangesAsync();

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