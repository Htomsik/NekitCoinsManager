using System.Collections.Generic;
using System.Threading.Tasks;
using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Core.Services;

/// <summary>
/// Интерфейс сервиса конвертации валют
/// </summary>
public interface ICurrencyConversionService
{
    /// <summary>
    /// Конвертирует сумму из одной валюты в другую
    /// </summary>
    /// <param name="amount">Сумма в исходной валюте</param>
    /// <param name="fromCurrencyCode">Код исходной валюты</param>
    /// <param name="toCurrencyCode">Код целевой валюты</param>
    /// <returns>Сконвертированная сумма в целевой валюте</returns>
    Task<decimal> ConvertAsync(decimal amount, string fromCurrencyCode, string toCurrencyCode);
    
    /// <summary>
    /// Получает текущий обменный курс между двумя валютами
    /// </summary>
    /// <param name="fromCurrencyCode">Код исходной валюты</param>
    /// <param name="toCurrencyCode">Код целевой валюты</param>
    /// <returns>Обменный курс</returns>
    Task<decimal> GetExchangeRateAsync(string fromCurrencyCode, string toCurrencyCode);
    
    /// <summary>
    /// Получает все доступные обменные курсы
    /// </summary>
    /// <returns>Словарь обменных курсов [fromCurrencyCode][toCurrencyCode] = rate</returns>
    Task<Dictionary<string, Dictionary<string, decimal>>> GetAllExchangeRatesAsync();
    
    /// <summary>
    /// Обновляет обменный курс между двумя валютами
    /// </summary>
    /// <param name="fromCurrencyCode">Код исходной валюты</param>
    /// <param name="toCurrencyCode">Код целевой валюты</param>
    /// <param name="rate">Новый обменный курс</param>
    /// <returns>Результат операции (успех/ошибка)</returns>
    Task<(bool success, string? error)> UpdateExchangeRateAsync(string fromCurrencyCode, string toCurrencyCode, decimal rate);
} 