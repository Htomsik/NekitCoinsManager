namespace NekitCoinsManager.Shared.DTO.Operations;

/// <summary>
/// DTO для операции перевода средств
/// </summary>
public class TransferDto
{
    /// <summary>
    /// ID пользователя, выполняющего перевод
    /// </summary>
    public int UserId { get; set; }
    
    /// <summary>
    /// ID пользователя-получателя
    /// </summary>
    public int RecipientId { get; set; }
    
    /// <summary>
    /// ID валюты
    /// </summary>
    public int CurrencyId { get; set; }
    
    /// <summary>
    /// Сумма перевода
    /// </summary>
    public decimal Amount { get; set; }
    
    /// <summary>
    /// Комментарий к переводу
    /// </summary>
    public string Comment { get; set; } = string.Empty;
} 