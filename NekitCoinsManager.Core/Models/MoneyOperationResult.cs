namespace NekitCoinsManager.Core.Models;

/// <summary>
/// Результат выполнения финансовой операции
/// </summary>
public class MoneyOperationResult
{
    /// <summary>
    /// Успешно ли выполнена операция
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Сообщение об ошибке (если операция не выполнена)
    /// </summary>
    public string? Error { get; set; }
    
    /// <summary>
    /// ID созданной транзакции (если операция выполнена успешно)
    /// </summary>
    public int? TransactionId { get; set; }
    
    /// <summary>
    /// Дополнительные данные результата операции
    /// </summary>
    public object? Data { get; set; }
    
    /// <summary>
    /// Создает успешный результат операции
    /// </summary>
    public static MoneyOperationResult CreateSuccess(int? transactionId = null, object? data = null)
    {
        return new MoneyOperationResult
        {
            Success = true,
            TransactionId = transactionId,
            Data = data
        };
    }
    
    /// <summary>
    /// Создает результат с ошибкой
    /// </summary>
    public static MoneyOperationResult CreateError(string error)
    {
        return new MoneyOperationResult
        {
            Success = false,
            Error = error
        };
    }
}