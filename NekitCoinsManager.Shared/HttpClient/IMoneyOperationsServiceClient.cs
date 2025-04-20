using System.Threading.Tasks;
using NekitCoinsManager.Shared.DTO;
using NekitCoinsManager.Shared.DTO.Operations;

namespace NekitCoinsManager.Shared.HttpClient;

/// <summary>
/// Клиентский интерфейс для денежных операций
/// </summary>
public interface IMoneyOperationsServiceClient
{
    /// <summary>
    /// Выполняет перевод денег между пользователями
    /// </summary>
    /// <param name="transferDto">Данные для перевода</param>
    /// <returns>Результат операции перевода</returns>
    Task<MoneyOperationResultDto> TransferAsync(TransferDto transferDto);
    
    /// <summary>
    /// Пополняет баланс пользователя (депозит)
    /// </summary>
    /// <param name="depositDto">Данные для пополнения</param>
    /// <returns>Результат операции пополнения</returns>
    Task<MoneyOperationResultDto> DepositAsync(DepositDto depositDto);
    
    /// <summary>
    /// Конвертирует средства пользователя из одной валюты в другую
    /// </summary>
    /// <param name="conversionDto">Данные для конвертации</param>
    /// <returns>Результат операции конвертации</returns>
    Task<MoneyOperationResultDto> ConvertAsync(ConversionDto conversionDto);

    /// <summary>
    /// Выдает приветственный бонус новому пользователю
    /// </summary>
    /// <param name="welcomeBonusDto">Данные для выдачи приветственного бонуса</param>
    /// <returns>Результат операции выдачи бонуса</returns>
    Task<MoneyOperationResultDto> GrantWelcomeBonusAsync(WelcomeBonusDto welcomeBonusDto);
} 