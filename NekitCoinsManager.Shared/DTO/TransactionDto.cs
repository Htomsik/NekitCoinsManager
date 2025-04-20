using System;

namespace NekitCoinsManager.Shared.DTO;

/// <summary>
/// Базовая DTO для транзакции без глубоких связей
/// </summary>
public class TransactionDto
{
    /// <summary>
    /// Идентификатор транзакции
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Идентификатор пользователя-отправителя
    /// </summary>
    public int FromUserId { get; set; }
    
    /// <summary>
    /// Идентификатор пользователя-получателя
    /// </summary>
    public int ToUserId { get; set; }
    
    /// <summary>
    /// Идентификатор валюты
    /// </summary>
    public int CurrencyId { get; set; }
    
    /// <summary>
    /// Сумма транзакции
    /// </summary>
    public decimal Amount { get; set; }
    
    /// <summary>
    /// Комментарий к транзакции
    /// </summary>
    public string Comment { get; set; } = string.Empty;
    
    /// <summary>
    /// Дата создания транзакции
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Тип транзакции
    /// </summary>
    public TransactionTypeDto Type { get; set; }
    
    /// <summary>
    /// ID родительской транзакции (если есть)
    /// </summary>
    public int? ParentTransactionId { get; set; }
    
    /// <summary>
    /// Информация о пользователе-отправителе (опционально)
    /// </summary>
    public UserDto? FromUser { get; set; }
    
    /// <summary>
    /// Информация о пользователе-получателе (опционально)
    /// </summary>
    public UserDto? ToUser { get; set; }
    
    /// <summary>
    /// Информация о валюте (опционально)
    /// </summary>
    public CurrencyDto? Currency { get; set; }
} 