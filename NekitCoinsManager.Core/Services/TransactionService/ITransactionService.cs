using System.Collections.Generic;
using System.Threading.Tasks;
using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Core.Services;

public interface ITransactionObserver
{
    void OnTransactionsChanged();
}

/// <summary>
/// Сервис для работы с транзакциями
/// </summary>
public interface ITransactionService
{
    /// <summary>
    /// Получает все транзакции
    /// </summary>
    Task<IEnumerable<Transaction>> GetTransactionsAsync();
    
    /// <summary>
    /// Получает транзакцию по идентификатору
    /// </summary>
    Task<Transaction?> GetTransactionByIdAsync(int id);
    
    /// <summary>
    /// Добавляет новую транзакцию
    /// </summary>
    Task<(bool success, string? error)> AddTransactionAsync(Transaction transaction);
    
    /// <summary>
    /// Валидирует транзакцию
    /// </summary>
    Task<(bool isValid, string? errorMessage)> ValidateTransactionAsync(Transaction transaction);
    
    /// <summary>
    /// Подписаться на обновление транзакций
    /// </summary>
    void Subscribe(ITransactionObserver observer);
    
    /// <summary>
    /// Уведомить наблюдателей о изменении транзакций
    /// </summary>
    void NotifyObservers();
} 