namespace NekitCoinsManager.Models;

/// <summary>
/// Модель представления валюты для отображения в интерфейсе
/// </summary>
public class CurrencyDisplayModel
{
    /// <summary>
    /// Идентификатор валюты
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Название валюты
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Код валюты (например, USD, EUR)
    /// </summary>
    public string Code { get; set; } = string.Empty;
    
    /// <summary>
    /// Символ валюты (например, $, €)
    /// </summary>
    public string Symbol { get; set; } = string.Empty;
} 