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
    public async Task<OperationResultDto> AddCurrencyAsync(CurrencyDto currency)
    {
        var currencyModel = _mapper.Map<NekitCoinsManager.Core.Models.Currency>(currency);
        var (success, error) = await _currencyService.AddCurrencyAsync(currencyModel);
        return success 
            ? OperationResultDto.CreateSuccess() 
            : OperationResultDto.CreateError(error ?? "Ошибка при добавлении валюты");
    }

    /// <inheritdoc />
    public async Task<OperationResultDto> UpdateCurrencyAsync(CurrencyDto currency)
    {
        var currencyModel = _mapper.Map<NekitCoinsManager.Core.Models.Currency>(currency);
        var (success, error) = await _currencyService.UpdateCurrencyAsync(currencyModel);
        return success 
            ? OperationResultDto.CreateSuccess() 
            : OperationResultDto.CreateError(error ?? "Ошибка при обновлении валюты");
    }

    /// <inheritdoc />
    public async Task<OperationResultDto> DeleteCurrencyAsync(int id)
    {
        var (success, error) = await _currencyService.DeleteCurrencyAsync(id);
        return success 
            ? OperationResultDto.CreateSuccess() 
            : OperationResultDto.CreateError(error ?? "Ошибка при удалении валюты");
    }

    /// <inheritdoc />
    public async Task<OperationResultDto> UpdateExchangeRateAsync(int currencyId, decimal newRate)
    {
        var (success, error) = await _currencyService.UpdateExchangeRateAsync(currencyId, newRate);
        return success 
            ? OperationResultDto.CreateSuccess() 
            : OperationResultDto.CreateError(error ?? "Ошибка при обновлении обменного курса");
    }
} 