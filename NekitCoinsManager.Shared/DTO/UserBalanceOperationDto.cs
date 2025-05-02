using System.ComponentModel.DataAnnotations;

namespace NekitCoinsManager.Shared.DTO;

/// <summary>
/// Базовый DTO для операций с балансом пользователя
/// (создание, обновление, проверка, валидация)
/// </summary>
public class UserBalanceModifyDto
{
    /// <summary>
    /// Идентификатор пользователя (отправителя при переводе)
    /// </summary>
    [Required]
    public int UserId { get; set; }
    
    /// <summary>
    /// Идентификатор валюты
    /// </summary>
    [Required]
    public int CurrencyId { get; set; }
    
    /// <summary>
    /// Сумма для операции
    /// </summary>
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Сумма должна быть положительным числом")]
    public decimal Amount { get; set; }
}

/// <summary>
/// DTO для перевода средств между балансами пользователей
/// </summary>
public class UserBalanceTransferDto : UserBalanceModifyDto
{
    /// <summary>
    /// Идентификатор пользователя-получателя
    /// </summary>
    [Required]
    public int ToUserId { get; set; }
} 