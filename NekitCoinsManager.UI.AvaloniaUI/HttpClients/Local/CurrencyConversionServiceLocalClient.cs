using System.Collections.Generic;
using System.Threading.Tasks;
using MapsterMapper;
using NekitCoinsManager.Core.Services;
using NekitCoinsManager.Shared.DTO;
using NekitCoinsManager.Shared.DTO.Operations;
using NekitCoinsManager.Shared.HttpClient;

namespace NekitCoinsManager.HttpClients;

/// <summary>
/// Локальная реализация клиента для конвертации валют
/// </summary>
public class CurrencyConversionServiceLocalClient : ICurrencyConversionServiceClient
{
    private readonly ICurrencyConversionService _currencyConversionService;
    private readonly IMapper _mapper;

    /// <summary>
    /// Создает экземпляр локального клиента для конвертации валют
    /// </summary>
    /// <param name="currencyConversionService">Оригинальный сервис конвертации валют</param>
    /// <param name="mapper">Инструмент для маппинга моделей</param>
    public CurrencyConversionServiceLocalClient(ICurrencyConversionService currencyConversionService, IMapper mapper)
    {
        _currencyConversionService = currencyConversionService;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<decimal> ConvertCurrencyAsync(CurrencyConversionDto conversionDto)
    {
        return await _currencyConversionService.ConvertAsync(
            conversionDto.Amount, 
            conversionDto.FromCurrencyCode, 
            conversionDto.ToCurrencyCode);
    }

    /// <inheritdoc />
    public async Task<decimal> GetExchangeRateAsync(CurrencyExchangeRateOperationDto queryDto)
    {
        return await _currencyConversionService.GetExchangeRateAsync(
            queryDto.FromCurrencyCode, 
            queryDto.ToCurrencyCode);
    }

    /// <inheritdoc />
    public async Task<Dictionary<string, Dictionary<string, decimal>>> GetAllExchangeRatesAsync()
    {
        return await _currencyConversionService.GetAllExchangeRatesAsync();
    }
} 