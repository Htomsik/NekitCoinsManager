using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Core.Services;

/// <summary>
/// Интерфейс сервиса денежных операций
/// </summary>
public interface IMoneyOperationsManager
{
    /// <summary>
    /// Выполняет перевод денег между пользователями
    /// </summary>
    /// <param name="transferData">Данные для перевода</param>
    /// <returns>Результат операции перевода</returns>
    Task<MoneyOperationResult> TransferAsync(TransferDto transferData);
    
    /// <summary>
    /// Пополняет баланс пользователя (депозит)
    /// </summary>
    /// <param name="depositData">Данные для пополнения</param>
    /// <returns>Результат операции пополнения</returns>
    Task<MoneyOperationResult> DepositAsync(DepositDto depositData);
    
    /// <summary>
    /// Конвертирует средства пользователя из одной валюты в другую
    /// </summary>
    /// <param name="conversionData">Данные для конвертации</param>
    /// <returns>Результат операции конвертации</returns>
    Task<MoneyOperationResult> ConvertAsync(ConversionDto conversionData);

    /// <summary>
    /// Выдает приветственный бонус новому пользователю
    /// </summary>
    /// <param name="welcomeBonusData">Данные для выдачи приветственного бонуса</param>
    /// <returns>Результат операции выдачи бонуса</returns>
    Task<MoneyOperationResult> GrantWelcomeBonusAsync(WelcomeBonusDto welcomeBonusData);
} 