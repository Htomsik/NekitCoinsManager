using System.Collections.Generic;
using System.Threading.Tasks;
using NekitCoinsManager.Shared.DTO;
using NekitCoinsManager.Shared.DTO.Operations;

namespace NekitCoinsManager.Shared.HttpClient;

/// <summary>
/// Клиентский интерфейс сервиса конвертации валют
/// </summary>
public interface ICurrencyConversionServiceClient
{
    /// <summary>
    /// Конвертирует сумму из одной валюты в другую
    /// </summary>
    /// <param name="conversionDto">Данные для конвертации</param>
    /// <returns>Сконвертированная сумма</returns>
    Task<decimal> ConvertCurrencyAsync(CurrencyConversionDto conversionDto);
    
    /// <summary>
    /// Получает текущий обменный курс между двумя валютами
    /// </summary>
    /// <param name="queryDto">Параметры запроса</param>
    /// <returns>Обменный курс</returns>
    Task<decimal> GetExchangeRateAsync(CurrencyExchangeRateOperationDto queryDto);
    
    /// <summary>
    /// Получает все доступные обменные курсы
    /// </summary>
    /// <returns>Словарь обменных курсов [fromCurrencyCode][toCurrencyCode] = rate</returns>
    Task<Dictionary<string, Dictionary<string, decimal>>> GetAllExchangeRatesAsync();
} 