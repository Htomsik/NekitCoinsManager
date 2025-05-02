namespace NekitCoinsManager.Shared.DTO;

/// <summary>
/// DTO с результатом получения обменного курса валют
/// </summary>
public class CurrencyExchangeRateResultDto
{
    /// <summary>
    /// Обменный курс между двумя валютами
    /// </summary>
    public decimal Rate { get; set; }
} 