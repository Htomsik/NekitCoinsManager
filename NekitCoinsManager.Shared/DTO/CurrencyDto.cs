using System;

namespace NekitCoinsManager.Shared.DTO;

/// <summary>
/// DTO модель валюты
/// </summary>
public class CurrencyDto
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
    /// Код валюты
    /// </summary>
    public string Code { get; set; } = string.Empty;
    
    /// <summary>
    /// Символ валюты
    /// </summary>
    public string Symbol { get; set; } = string.Empty;
    
    /// <summary>
    /// Обменный курс
    /// </summary>
    public decimal ExchangeRate { get; set; }
    
    /// <summary>
    /// Время последнего обновления
    /// </summary>
    public DateTime LastUpdateTime { get; set; }
    
    /// <summary>
    /// Признак активности валюты
    /// </summary>
    public bool IsActive { get; set; }
    
    /// <summary>
    /// Признак того, что валюта начисляется новым пользователям при регистрации
    /// </summary>
    public bool IsDefaultForNewUsers { get; set; }
    
    /// <summary>
    /// Количество валюты, начисляемое при регистрации
    /// </summary>
    public decimal DefaultAmount { get; set; }
    
    /// <summary>
    /// Процент комиссии при конвертации из этой валюты
    /// </summary>
    public decimal ConversionFeePercentage { get; set; }
} 