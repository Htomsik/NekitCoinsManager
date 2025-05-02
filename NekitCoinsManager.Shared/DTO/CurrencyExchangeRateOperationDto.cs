namespace NekitCoinsManager.Shared.DTO;

/// <summary>
/// DTO для запроса обменного курса между валютами
/// </summary>
public class CurrencyExchangeRateOperationDto
{
    /// <summary>
    /// Код исходной валюты
    /// </summary>
    public string FromCurrencyCode { get; set; } = string.Empty;
    
    /// <summary>
    /// Код целевой валюты
    /// </summary>
    public string ToCurrencyCode { get; set; } = string.Empty;
} 