using System;

namespace NekitCoinsManager.Shared.DTO;

/// <summary>
/// DTO для баланса пользователя
/// </summary>
public class UserBalanceDto
{
    /// <summary>
    /// Идентификатор баланса
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public int UserId { get; set; }
    
    /// <summary>
    /// Идентификатор валюты
    /// </summary>
    public int CurrencyId { get; set; }
    
    /// <summary>
    /// Сумма на балансе
    /// </summary>
    public decimal Amount { get; set; }
    
    /// <summary>
    /// Время последнего обновления
    /// </summary>
    public DateTime LastUpdateTime { get; set; }
    
    /// <summary>
    /// Информация о валюте (опционально)
    /// </summary>
    public CurrencyDto? Currency { get; set; }
} 