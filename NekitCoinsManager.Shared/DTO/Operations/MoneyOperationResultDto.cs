namespace NekitCoinsManager.Shared.DTO.Operations;

/// <summary>
/// Результат выполнения операции
/// </summary>
public class MoneyOperationResultDto
{
    /// <summary>
    /// Признак успешного выполнения операции
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// Сообщение об ошибке (если операция не выполнена)
    /// </summary>
    public string? Error { get; set; }
    
    /// <summary>
    /// Код ошибки (если операция не выполнена)
    /// </summary>
    public int? ErrorCode { get; set; }
    
    /// <summary>
    /// Дополнительные данные результата операции (опционально)
    /// </summary>
    public object? Data { get; set; }
} 