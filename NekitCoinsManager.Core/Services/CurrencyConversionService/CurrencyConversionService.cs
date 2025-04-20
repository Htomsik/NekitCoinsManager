using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Core.Services;

/// <summary>
/// Сервис для конвертации валют
/// </summary>
public class CurrencyConversionService : ICurrencyConversionService
{
    private readonly ICurrencyService _currencyService;

    public CurrencyConversionService(ICurrencyService currencyService)
    {
        _currencyService = currencyService;
    }

    /// <summary>
    /// Конвертирует сумму из одной валюты в другую по коду валюты
    /// </summary>
    public async Task<decimal> ConvertAsync(decimal amount, string fromCurrencyCode, string toCurrencyCode)
    {
        // Если валюты совпадают, возвращаем исходную сумму
        if (fromCurrencyCode.Equals(toCurrencyCode, StringComparison.OrdinalIgnoreCase))
        {
            return amount;
        }

        // Получаем курс обмена
        var rate = await GetExchangeRateAsync(fromCurrencyCode, toCurrencyCode);
        
        // Выполняем конвертацию
        return amount * rate;
    }

    /// <summary>
    /// Получает курс обмена между двумя валютами
    /// </summary>
    public async Task<decimal> GetExchangeRateAsync(string fromCurrencyCode, string toCurrencyCode)
    {
        // Если валюты совпадают, курс равен 1
        if (fromCurrencyCode.Equals(toCurrencyCode, StringComparison.OrdinalIgnoreCase))
        {
            return 1m;
        }

        // Получаем валюты из сервиса
        var fromCurrency = await _currencyService.GetCurrencyByCodeAsync(fromCurrencyCode);
        var toCurrency = await _currencyService.GetCurrencyByCodeAsync(toCurrencyCode);
        
        if (fromCurrency == null)
            throw new ArgumentException($"Валюта с кодом {fromCurrencyCode} не найдена", nameof(fromCurrencyCode));
        
        if (toCurrency == null)
            throw new ArgumentException($"Валюта с кодом {toCurrencyCode} не найдена", nameof(toCurrencyCode));

        // Обменный курс = (курс второй валюты) / (курс первой валюты)
        decimal rate = toCurrency.ExchangeRate / fromCurrency.ExchangeRate;
        
        return rate;
    }

    /// <summary>
    /// Получает все доступные обменные курсы в виде словаря
    /// </summary>
    public async Task<Dictionary<string, Dictionary<string, decimal>>> GetAllExchangeRatesAsync()
    {
        // Получаем все валюты
        var currencies = await _currencyService.GetCurrenciesAsync();
        var currencyList = new List<Currency>(currencies);
        
        // Создаем словарь для хранения всех курсов
        var result = new Dictionary<string, Dictionary<string, decimal>>();
        
        // Заполняем словарь курсами
        foreach (var fromCurrency in currencyList)
        {
            var rates = new Dictionary<string, decimal>();
            
            foreach (var toCurrency in currencyList)
            {
                // Рассчитываем курс как соотношение ExchangeRate валют
                decimal rate = toCurrency.ExchangeRate / fromCurrency.ExchangeRate;
                rates[toCurrency.Code] = rate;
            }
            
            result[fromCurrency.Code] = rates;
        }
        
        return result;
    }

    /// <summary>
    /// Обновляет обменный курс между двумя валютами
    /// </summary>
    public async Task<(bool success, string? error)> UpdateExchangeRateAsync(string fromCurrencyCode, string toCurrencyCode, decimal rate)
    {
        // Валидация входных данных
        if (rate <= 0)
        {
            return (false, "Курс обмена должен быть больше нуля");
        }
        
        try
        {
            // Получаем валюты
            var fromCurrency = await _currencyService.GetCurrencyByCodeAsync(fromCurrencyCode);
            var toCurrency = await _currencyService.GetCurrencyByCodeAsync(toCurrencyCode);
            
            if (fromCurrency == null)
                return (false, $"Валюта с кодом {fromCurrencyCode} не найдена");
            
            if (toCurrency == null)
                return (false, $"Валюта с кодом {toCurrencyCode} не найдена");

            // Рассчитываем новое значение ExchangeRate для целевой валюты
            // Учитывая, что курс = toCurrency.ExchangeRate / fromCurrency.ExchangeRate
            // Тогда toCurrency.ExchangeRate = rate * fromCurrency.ExchangeRate
            decimal newExchangeRate = rate * fromCurrency.ExchangeRate;
            
            // Обновляем курс через CurrencyService
            var updateResult = await _currencyService.UpdateExchangeRateAsync(toCurrency.Id, newExchangeRate);
            
            return updateResult;
        }
        catch (Exception ex)
        {
            return (false, $"Ошибка при обновлении курса: {ex.Message}");
        }
    }
} 