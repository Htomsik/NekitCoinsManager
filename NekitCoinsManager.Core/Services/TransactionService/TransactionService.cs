using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NekitCoinsManager.Core.Data;
using NekitCoinsManager.Core.Models;
using NekitCoinsManager.Core.Repositories;

namespace NekitCoinsManager.Core.Services;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrencyRepository _currencyRepository;
    private readonly List<ITransactionObserver> _observers = new();

    public TransactionService(
        ITransactionRepository transactionRepository,
        IUserRepository userRepository,
        ICurrencyRepository currencyRepository)
    {
        _transactionRepository = transactionRepository;
        _userRepository = userRepository;
        _currencyRepository = currencyRepository;
    }

    public async Task<IEnumerable<Transaction>> GetTransactionsAsync()
    {
        return await _transactionRepository.GetAllAsync();
    }

    public async Task<Transaction?> GetTransactionByIdAsync(int id)
    {
        return await _transactionRepository.GetByIdAsync(id);
    }

    public async Task<(bool success, string? error)> AddTransactionAsync(Transaction transaction)
    {
        // Валидируем транзакцию
        var (isValid, errorMessage) = await ValidateTransactionAsync(transaction);
        if (!isValid)
        {
            return (false, errorMessage);
        }

        // Устанавливаем время создания, если не задано
        if (transaction.CreatedAt == default)
        {
            transaction.CreatedAt = DateTime.UtcNow;
        }

        await _transactionRepository.AddAsync(transaction);
        NotifyObservers();
        return (true, null);
    }

    public async Task<(bool isValid, string? errorMessage)> ValidateTransactionAsync(Transaction transaction)
    {
        // Для операции пополнения (депозита) не проверяем отправителя
        bool isDeposit = transaction.Type == TransactionType.Deposit;

        // Проверяем валидность ID пользователей и валюты
        if (!isDeposit && transaction.FromUserId <= 0)
        {
            return (false, "Не указан отправитель перевода");
        }

        if (transaction.ToUserId <= 0)
        {
            return (false, "Не указан получатель перевода");
        }

        if (transaction.CurrencyId <= 0)
        {
            return (false, "Не указана валюта перевода");
        }

        if (transaction.Amount <= 0)
        {
            return (false, "Сумма перевода должна быть больше нуля");
        }

        // Проверяем существование пользователей и валюты в базе данных
        if (!isDeposit)
        {
            var fromUser = await _userRepository.GetByIdAsync(transaction.FromUserId);
            if (fromUser == null)
            {
                return (false, $"Отправитель с ID {transaction.FromUserId} не найден");
            }
        }

        var toUser = await _userRepository.GetByIdAsync(transaction.ToUserId);
        if (toUser == null)
        {
            return (false, $"Получатель с ID {transaction.ToUserId} не найден");
        }

        var currency = await _currencyRepository.GetByIdAsync(transaction.CurrencyId);
        if (currency == null)
        {
            return (false, $"Валюта с ID {transaction.CurrencyId} не найдена");
        }

        // Проверяем тип транзакции
        if (!Enum.IsDefined(typeof(TransactionType), transaction.Type))
        {
            return (false, "Неверный тип транзакции");
        }

        // Специфические проверки для разных типов транзакций
        switch (transaction.Type)
        {
            case TransactionType.Transfer:
                if (transaction.FromUserId == transaction.ToUserId)
                {
                    return (false, "Нельзя переводить монеты самому себе");
                }
                break;
                
            case TransactionType.Deposit:
                // Для депозита не проверяем отправителя
                break;
                
            case TransactionType.Conversion:
                // Для конвертации допускается перевод самому себе
                break;
        }

        return (true, null);
    }

    public void Subscribe(ITransactionObserver observer)
    {
        if (!_observers.Contains(observer))
        {
            _observers.Add(observer);
        }
    }

    public void NotifyObservers()
    {
        foreach (var observer in _observers)
        {
            observer.OnTransactionsChanged();
        }
    }
} 