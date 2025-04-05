namespace NekitCoinsManager.Shared.DTO.Operations;

/// <summary>
/// Результат выполнения операции
/// </summary>
public class OperationResultDto
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
    
    /// <summary>
    /// Создать успешный результат операции
    /// </summary>
    public static OperationResultDto CreateSuccess(object? data = null)
    {
        return new OperationResultDto
        {
            Success = true,
            Data = data
        };
    }
    
    /// <summary>
    /// Создать результат операции с ошибкой
    /// </summary>
    public static OperationResultDto CreateError(string error, int? errorCode = null)
    {
        return new OperationResultDto
        {
            Success = false,
            Error = error,
            ErrorCode = errorCode
        };
    }
} 