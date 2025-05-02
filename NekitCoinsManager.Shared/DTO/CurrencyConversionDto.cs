namespace NekitCoinsManager.Shared.DTO;

/// <summary>
/// DTO для конвертации валюты (использует коды валют)
/// </summary>
public class CurrencyConversionDto
{
    /// <summary>
    /// Сумма в исходной валюте
    /// </summary>
    public decimal Amount { get; set; }
    
    /// <summary>
    /// Код исходной валюты
    /// </summary>
    public string FromCurrencyCode { get; set; } = string.Empty;
    
    /// <summary>
    /// Код целевой валюты
    /// </summary>
    public string ToCurrencyCode { get; set; } = string.Empty;
}