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
    /// <param name="transferOperationData">Данные для перевода</param>
    /// <returns>Результат операции перевода</returns>
    Task<MoneyOperationResult> TransferAsync(TransferOperation transferOperationData);
    
    /// <summary>
    /// Пополняет баланс пользователя (депозит)
    /// </summary>
    /// <param name="depositOperationData">Данные для пополнения</param>
    /// <returns>Результат операции пополнения</returns>
    Task<MoneyOperationResult> DepositAsync(DepositOperation depositOperationData);
    
    /// <summary>
    /// Конвертирует средства пользователя из одной валюты в другую
    /// </summary>
    /// <param name="conversionOperationData">Данные для конвертации</param>
    /// <returns>Результат операции конвертации</returns>
    Task<MoneyOperationResult> ConvertAsync(ConversionOperation conversionOperationData);

    /// <summary>
    /// Выдает приветственный бонус новому пользователю
    /// </summary>
    /// <param name="welcomeBonusOperationData">Данные для выдачи приветственного бонуса</param>
    /// <returns>Результат операции выдачи бонуса</returns>
    Task<MoneyOperationResult> GrantWelcomeBonusAsync(WelcomeBonusOperation welcomeBonusOperationData);
} 