using System.Collections.Generic;
using System.Threading.Tasks;
using MapsterMapper;
using NekitCoinsManager.Core.Services;
using NekitCoinsManager.Shared.DTO;
using NekitCoinsManager.Shared.DTO.Operations;
using NekitCoinsManager.Shared.HttpClient;

namespace NekitCoinsManager.HttpClients;

/// <summary>
/// Локальная реализация клиента для работы с валютами
/// </summary>
public class CurrencyServiceLocalClient : ICurrencyServiceClient
{
    private readonly ICurrencyService _currencyService;
    private readonly IMapper _mapper;

    /// <summary>
    /// Создает экземпляр локального клиента для работы с валютами
    /// </summary>
    /// <param name="currencyService">Оригинальный сервис для работы с валютами</param>
    /// <param name="mapper">Инструмент для маппинга моделей</param>
    public CurrencyServiceLocalClient(ICurrencyService currencyService, IMapper mapper)
    {
        _currencyService = currencyService;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<CurrencyDto>> GetCurrenciesAsync()
    {
        var currencies = await _currencyService.GetCurrenciesAsync();
        return _mapper.Map<IEnumerable<CurrencyDto>>(currencies);
    }

    /// <inheritdoc />
    public async Task<CurrencyDto?> GetCurrencyByIdAsync(int id)
    {
        var currency = await _currencyService.GetCurrencyByIdAsync(id);
        return currency != null ? _mapper.Map<CurrencyDto>(currency) : null;
    }

    /// <inheritdoc />
    public async Task<CurrencyDto?> GetCurrencyByCodeAsync(string code)
    {
        var currency = await _currencyService.GetCurrencyByCodeAsync(code);
        return currency != null ? _mapper.Map<CurrencyDto>(currency) : null;
    }

    /// <inheritdoc />
    public async Task<(bool success, string? error)> AddCurrencyAsync(CurrencyDto currency)
    {
        var currencyModel = _mapper.Map<NekitCoinsManager.Core.Models.Currency>(currency);
        return await _currencyService.AddCurrencyAsync(currencyModel);
    }

    /// <inheritdoc />
    public async Task<(bool success, string? error)> UpdateCurrencyAsync(CurrencyDto currency)
    {
        var currencyModel = _mapper.Map<NekitCoinsManager.Core.Models.Currency>(currency);
        return await _currencyService.UpdateCurrencyAsync(currencyModel);
    }

    /// <inheritdoc />
    public async Task<(bool success, string? error)> DeleteCurrencyAsync(int id)
    {
        return await _currencyService.DeleteCurrencyAsync(id);
    }

    /// <inheritdoc />
    public async Task<(bool success, string? error)> UpdateExchangeRateAsync(int currencyId, decimal newRate)
    {
        return await _currencyService.UpdateExchangeRateAsync(currencyId, newRate);
    }
} 