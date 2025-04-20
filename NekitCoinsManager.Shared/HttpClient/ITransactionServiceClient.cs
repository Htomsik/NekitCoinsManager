using System.Collections.Generic;
using System.Threading.Tasks;
using NekitCoinsManager.Shared.DTO;

namespace NekitCoinsManager.Shared.HttpClient;

public interface ITransactionObserverClient
{
    void OnTransactionsChanged();
}

/// <summary>
/// Клиентский интерфейс для работы с транзакциями
/// </summary>
public interface ITransactionServiceClient
{
    /// <summary>
    /// Получает все транзакции
    /// </summary>
    Task<IEnumerable<TransactionDto>> GetTransactionsAsync();
    
    /// <summary>
    /// Получает транзакцию по идентификатору
    /// </summary>
    Task<TransactionDto?> GetTransactionByIdAsync(int id);
    
    /// <summary>
    /// Добавляет новую транзакцию
    /// </summary>
    Task<(bool success, string? error)> AddTransactionAsync(TransactionDto transaction);
    
    /// <summary>
    /// Валидирует транзакцию
    /// </summary>
    Task<(bool isValid, string? errorMessage)> ValidateTransactionAsync(TransactionDto transaction);
    
    /// <summary>
    /// Подписаться на обновление транзакций
    /// </summary>
    void Subscribe(ITransactionObserverClient observer);
    
    /// <summary>
    /// Уведомить наблюдателей о изменении транзакций
    /// </summary>
    void NotifyObservers();
} 