using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using NekitCoinsManager.Core.Services;
using NekitCoinsManager.Shared.DTO;
using NekitCoinsManager.Shared.DTO.Operations;

namespace NekitCoinsManager.API.Controllers;

/// <summary>
/// Контроллер для работы с конвертацией валют
/// </summary>
public class CurrencyConversionController : BaseApiController
{
    private readonly ICurrencyConversionService _currencyConversionService;
    
    /// <summary>
    /// Конструктор контроллера
    /// </summary>
    public CurrencyConversionController(
        ICurrencyConversionService currencyConversionService,
        IMapper mapper) : base(mapper)
    {
        _currencyConversionService = currencyConversionService;
    }
    
    /// <summary>
    /// Конвертирует сумму из одной валюты в другую
    /// </summary>
    /// <param name="conversionDto">Данные для конвертации</param>
    /// <returns>Сконвертированная сумма</returns>
    [HttpPost("convert")]
    public async Task<IActionResult> ConvertCurrency([FromBody] CurrencyConversionDto conversionDto)
    {
        var convertedAmount = await _currencyConversionService.ConvertAsync(
            conversionDto.Amount, 
            conversionDto.FromCurrencyCode, 
            conversionDto.ToCurrencyCode);
            
        var result = new CurrencyConversionResultDto 
        { 
            Amount = convertedAmount
        };
        
        return Ok(result);
    }
    
    /// <summary>
    /// Получает обменный курс между двумя валютами
    /// </summary>
    /// <param name="queryDto">Параметры запроса</param>
    /// <returns>Обменный курс</returns>
    [HttpGet("rate")]
    public async Task<IActionResult> GetExchangeRate([FromQuery] CurrencyExchangeRateOperationDto queryDto)
    {
        var rate = await _currencyConversionService.GetExchangeRateAsync(
            queryDto.FromCurrencyCode, 
            queryDto.ToCurrencyCode);
            
        var result = new CurrencyExchangeRateResultDto { Rate = rate };
        
        return Ok(result);
    }
    
    /// <summary>
    /// Получает все доступные обменные курсы
    /// </summary>
    /// <returns>Словарь обменных курсов</returns>
    [HttpGet("rates")]
    public async Task<IActionResult> GetAllExchangeRates()
    {
        var rates = await _currencyConversionService.GetAllExchangeRatesAsync();
        var result = new CurrencyExchangeRatesDto { Rates = rates };
        return Ok(result);
    }
} 