using System.Threading.Tasks;
using NekitCoinsManager.Shared.DTO;
using NekitCoinsManager.Shared.DTO.Operations;

namespace NekitCoinsManager.Shared.HttpClient;

/// <summary>
/// Интерфейс наблюдателя за операциями с деньгами
/// </summary>
public interface IMoneyOperationsObserverClient
{
    /// <summary>
    /// Вызывается при изменении операций с деньгами
    /// </summary>
    void OnMoneyOperationsChanged();
}

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
    /// Подписаться на обновления операций с деньгами
    /// </summary>
    /// <param name="observer">Наблюдатель операций</param>
    void Subscribe(IMoneyOperationsObserverClient observer);
    
    /// <summary>
    /// Отписаться от обновлений операций с деньгами
    /// </summary>
    /// <param name="observer">Наблюдатель операций</param>
    void Unsubscribe(IMoneyOperationsObserverClient observer);
    
    /// <summary>
    /// Уведомить наблюдателей об изменениях
    /// </summary>
    void NotifyObservers();
} 