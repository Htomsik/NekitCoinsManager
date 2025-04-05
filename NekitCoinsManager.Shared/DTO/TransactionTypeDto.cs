namespace NekitCoinsManager.Shared.DTO;

/// <summary>
/// Типы транзакций в системе
/// </summary>
public enum TransactionTypeDto
{
    /// <summary>
    /// Перевод между пользователями
    /// </summary>
    Transfer = 0,
    
    /// <summary>
    /// Пополнение счета 
    /// </summary>
    Deposit = 1,
    
    /// <summary>
    /// Конвертация валюты
    /// </summary>
    Conversion = 2,
    
    /// <summary>
    /// Комиссия за операцию
    /// </summary>
    Fee = 3
} 