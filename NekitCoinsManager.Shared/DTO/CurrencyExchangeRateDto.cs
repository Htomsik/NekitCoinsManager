using System.ComponentModel.DataAnnotations;

namespace NekitCoinsManager.Shared.DTO;

/// <summary>
/// DTO для обновления курса обмена валюты
/// </summary>
public class CurrencyExchangeRateDto
{
    /// <summary>
    /// Идентификатор валюты
    /// </summary>
    [Required]
    public int Id { get; set; }
    
    /// <summary>
    /// Новый курс обмена валюты
    /// </summary>
    [Required]
    [Range(0.000001, double.MaxValue, ErrorMessage = "Курс обмена должен быть положительным числом")]
    public decimal ExchangeRate { get; set; }
} 