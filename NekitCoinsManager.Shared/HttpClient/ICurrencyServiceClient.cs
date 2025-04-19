using System.Collections.Generic;
using System.Threading.Tasks;
using NekitCoinsManager.Shared.DTO;
using NekitCoinsManager.Shared.DTO.Operations;

namespace NekitCoinsManager.Shared.HttpClient;

/// <summary>
/// Клиентский интерфейс для работы с валютами
/// </summary>
public interface ICurrencyServiceClient
{
    /// <summary>
    /// Получает список всех валют
    /// </summary>
    /// <returns>Список валют</returns>
    Task<IEnumerable<CurrencyDto>> GetCurrenciesAsync();
    
    /// <summary>
    /// Получает валюту по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор валюты</param>
    /// <returns>Валюта или null, если не найдена</returns>
    Task<CurrencyDto?> GetCurrencyByIdAsync(int id);
    
    /// <summary>
    /// Получает валюту по коду
    /// </summary>
    /// <param name="code">Код валюты</param>
    /// <returns>Валюта или null, если не найдена</returns>
    Task<CurrencyDto?> GetCurrencyByCodeAsync(string code);
    
    /// <summary>
    /// Добавляет новую валюту
    /// </summary>
    /// <param name="currency">Данные валюты</param>
    /// <returns>Результат операции</returns>
    Task<(bool success, string? error)> AddCurrencyAsync(CurrencyDto currency);
    
    /// <summary>
    /// Обновляет данные валюты
    /// </summary>
    /// <param name="currency">Обновленные данные валюты</param>
    /// <returns>Результат операции</returns>
    Task<(bool success, string? error)> UpdateCurrencyAsync(CurrencyDto currency);
    
    /// <summary>
    /// Удаляет валюту
    /// </summary>
    /// <param name="id">Идентификатор валюты</param>
    /// <returns>Результат операции</returns>
    Task<(bool success, string? error)> DeleteCurrencyAsync(int id);
    
    /// <summary>
    /// Обновляет обменный курс валюты
    /// </summary>
    /// <param name="currencyId">Идентификатор валюты</param>
    /// <param name="newRate">Новый обменный курс</param>
    /// <returns>Результат операции</returns>
    Task<(bool success, string? error)> UpdateExchangeRateAsync(int currencyId, decimal newRate);
} 