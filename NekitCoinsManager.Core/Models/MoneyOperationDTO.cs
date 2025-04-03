using System;

namespace NekitCoinsManager.Core.Models;

/// <summary>
/// Базовый DTO для финансовых операций
/// </summary>
public abstract class MoneyOperationDto
{
    /// <summary>
    /// ID пользователя, который инициировал операцию
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// ID валюты для операции
    /// </summary>
    public int CurrencyId { get; set; }

    /// <summary>
    /// Сумма операции
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Комментарий к операции
    /// </summary>
    public string Comment { get; set; } = string.Empty;
}

/// <summary>
/// DTO для операции перевода денег между пользователями
/// </summary>
public class TransferDto : MoneyOperationDto
{
    /// <summary>
    /// ID получателя перевода
    /// </summary>
    public int RecipientId { get; set; }
}

/// <summary>
/// DTO для операции пополнения баланса
/// </summary>
public class DepositDto : MoneyOperationDto
{
    // Для пополнения используем только базовые поля
}

/// <summary>
/// DTO для операции конвертации валюты
/// </summary>
public class ConversionDto : MoneyOperationDto
{
    /// <summary>
    /// ID целевой валюты для конвертации
    /// </summary>
    public int TargetCurrencyId { get; set; }
}

/// <summary>
/// DTO для операции выдачи приветственного бонуса
/// </summary>
public class WelcomeBonusDto : MoneyOperationDto
{
    /// <summary>
    /// ID нового пользователя, который получит бонус
    /// </summary>
    public int NewUserId { get; set; }
}

