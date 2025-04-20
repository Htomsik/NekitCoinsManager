namespace NekitCoinsManager.Shared.DTO.Operations;

/// <summary>
/// DTO для операции конвертации валют
/// </summary>
public class ConversionDto
{
    /// <summary>
    /// ID пользователя, выполняющего конвертацию
    /// </summary>
    public int UserId { get; set; }
    
    /// <summary>
    /// ID исходной валюты
    /// </summary>
    public int CurrencyId { get; set; }
    
    /// <summary>
    /// ID целевой валюты
    /// </summary>
    public int TargetCurrencyId { get; set; }
    
    /// <summary>
    /// Сумма конвертации (в исходной валюте)
    /// </summary>
    public decimal Amount { get; set; }
    
    /// <summary>
    /// Комментарий к конвертации
    /// </summary>
    public string Comment { get; set; } = string.Empty;
} 