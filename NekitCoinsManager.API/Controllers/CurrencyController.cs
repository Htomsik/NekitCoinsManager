using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using NekitCoinsManager.Core.Services;
using NekitCoinsManager.Shared.DTO;

namespace NekitCoinsManager.API.Controllers;

/// <summary>
/// Контроллер для работы с валютами
/// </summary>
public class CurrencyController : BaseApiController
{
    private readonly ICurrencyService _currencyService;

    /// <summary>
    /// Конструктор контроллера
    /// </summary>
    public CurrencyController(
        ICurrencyService currencyService,
        IMapper mapper) : base(mapper)
    {
        _currencyService = currencyService;
    }

    /// <summary>
    /// Получает список всех валют
    /// </summary>
    /// <returns>Список валют</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CurrencyDto>>> GetCurrencies()
    {
        var currencies = await _currencyService.GetCurrenciesAsync();
        return Ok(Mapper.Map<List<CurrencyDto>>(currencies));
    }

    /// <summary>
    /// Получает валюту по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор валюты</param>
    /// <returns>Валюта или ошибка, если не найдена</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<CurrencyDto>> GetCurrencyById(int id)
    {
        var currency = await _currencyService.GetCurrencyByIdAsync(id);
        if (currency == null)
            return NotFound(new { error = "Валюта не найдена" });

        return Ok(Mapper.Map<CurrencyDto>(currency));
    }

    /// <summary>
    /// Получает валюту по коду
    /// </summary>
    /// <param name="code">Код валюты</param>
    /// <returns>Валюта или ошибка, если не найдена</returns>
    [HttpGet("byCode/{code}")]
    public async Task<ActionResult<CurrencyDto>> GetCurrencyByCode(string code)
    {
        var currency = await _currencyService.GetCurrencyByCodeAsync(code);
        if (currency == null)
            return NotFound(new { error = "Валюта не найдена" });

        return Ok(Mapper.Map<CurrencyDto>(currency));
    }

    /// <summary>
    /// Добавляет новую валюту
    /// </summary>
    /// <param name="currencyDto">Данные валюты</param>
    /// <returns>Результат операции</returns>
    [HttpPost]
    public async Task<IActionResult> AddCurrency([FromBody] CurrencyDto currencyDto)
    {
        var currency = Mapper.Map<Core.Models.Currency>(currencyDto);
        
        var (success, error) = await _currencyService.AddCurrencyAsync(currency);
        if (!success)
            return BadRequest(new { error });

        return Ok(new { message = "Валюта успешно добавлена" });
    }

    /// <summary>
    /// Обновляет данные валюты
    /// </summary>
    /// <param name="currencyDto">Данные для обновления</param>
    /// <returns>Результат операции</returns>
    [HttpPut]
    public async Task<IActionResult> UpdateCurrency([FromBody] CurrencyDto currencyDto)
    {
        // Преобразуем DTO в модель и передаем сервису
        var currency = Mapper.Map<Core.Models.Currency>(currencyDto);
        
        var (success, error) = await _currencyService.UpdateCurrencyAsync(currency);
        if (!success)
            return BadRequest(new { error });

        return Ok(new { message = "Валюта успешно обновлена" });
    }

    /// <summary>
    /// Удаляет валюту
    /// </summary>
    /// <param name="id">Идентификатор валюты</param>
    /// <returns>Результат операции</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCurrency(int id)
    {
        var (success, error) = await _currencyService.DeleteCurrencyAsync(id);
        if (!success)
            return BadRequest(new { error });

        return Ok(new { message = "Валюта успешно удалена" });
    }

    /// <summary>
    /// Обновляет обменный курс валюты
    /// </summary>
    /// <param name="rateInfo">Данные для обновления курса</param>
    /// <returns>Результат операции</returns>
    [HttpPatch("rate")]
    public async Task<IActionResult> UpdateExchangeRate([FromBody] CurrencyExchangeRateDto rateInfo)
    {
        var (success, error) = await _currencyService.UpdateExchangeRateAsync(rateInfo.Id, rateInfo.ExchangeRate);
        if (!success)
            return BadRequest(new { error });

        return Ok(new { message = "Курс обмена успешно обновлен" });
    }
} 