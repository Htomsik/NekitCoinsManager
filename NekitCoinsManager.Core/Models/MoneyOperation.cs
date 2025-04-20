using System;

namespace NekitCoinsManager.Core.Models;

/// <summary>
/// Базовый DTO для финансовых операций
/// </summary>
public abstract class MoneyOperation
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
public class TransferOperation : MoneyOperation
{
    /// <summary>
    /// ID получателя перевода
    /// </summary>
    public int RecipientId { get; set; }
}

/// <summary>
/// DTO для операции пополнения баланса
/// </summary>
public class DepositOperation : MoneyOperation
{
    // Для пополнения используем только базовые поля
}

/// <summary>
/// DTO для операции конвертации валюты
/// </summary>
public class ConversionOperation : MoneyOperation
{
    /// <summary>
    /// ID целевой валюты для конвертации
    /// </summary>
    public int TargetCurrencyId { get; set; }
}

/// <summary>
/// DTO для операции выдачи приветственного бонуса
/// </summary>
public class WelcomeBonusOperation : MoneyOperation
{
    /// <summary>
    /// ID нового пользователя, который получит бонус
    /// </summary>
    public int NewUserId { get; set; }
}

