namespace NekitCoinsManager.Models;

/// <summary>
/// Модель ответа с ошибкой от API
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Сообщение об ошибке
    /// </summary>
    public string? Error { get; set; }
}
