namespace NekitCoinsManager.Shared.DTO.Operations;

/// <summary>
/// DTO для операции пополнения баланса
/// </summary>
public class DepositDto
{
    /// <summary>
    /// ID пользователя, которому пополняется баланс
    /// </summary>
    public int UserId { get; set; }
    
    /// <summary>
    /// ID валюты
    /// </summary>
    public int CurrencyId { get; set; }
    
    /// <summary>
    /// Сумма пополнения
    /// </summary>
    public decimal Amount { get; set; }
    
    /// <summary>
    /// Комментарий к пополнению
    /// </summary>
    public string Comment { get; set; } = string.Empty;
} 