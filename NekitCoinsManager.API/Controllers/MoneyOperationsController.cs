using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using NekitCoinsManager.Core.Services;
using NekitCoinsManager.Shared.DTO.Operations;

namespace NekitCoinsManager.API.Controllers;

/// <summary>
/// Контроллер для управления денежными операциями
/// </summary>
public class MoneyOperationsController : BaseApiController
{
    private readonly IMoneyOperationService<Core.Models.TransferOperation> _transferService;
    private readonly IMoneyOperationService<Core.Models.DepositOperation> _depositService;
    private readonly IMoneyOperationService<Core.Models.ConversionOperation> _conversionService;
    private readonly IMoneyOperationService<Core.Models.WelcomeBonusOperation> _welcomeBonusService;

    /// <summary>
    /// Конструктор контроллера
    /// </summary>
    public MoneyOperationsController(
        IMoneyOperationService<Core.Models.TransferOperation> transferService,
        IMoneyOperationService<Core.Models.DepositOperation> depositService,
        IMoneyOperationService<Core.Models.ConversionOperation> conversionService,
        IMoneyOperationService<Core.Models.WelcomeBonusOperation> welcomeBonusService,
        IMapper mapper) : base(mapper)
    {
        _transferService = transferService;
        _depositService = depositService;
        _conversionService = conversionService;
        _welcomeBonusService = welcomeBonusService;
    }

    /// <summary>
    /// Выполняет перевод средств между пользователями
    /// </summary>
    /// <param name="transferDto">Данные для перевода</param>
    /// <returns>Результат операции</returns>
    [HttpPost("transfer")]
    public async Task<IActionResult> TransferMoney([FromBody] TransferDto transferDto)
    {
        var operation = Mapper.Map<Core.Models.TransferOperation>(transferDto);
        var result = await _transferService.ExecuteAsync(operation);
        
        if (!result.Success)
            return BadRequest(new { error = result.Error });
            
        return Ok(Mapper.Map<MoneyOperationResultDto>(result));
    }
    
    /// <summary>
    /// Выполняет пополнение баланса пользователя
    /// </summary>
    /// <param name="depositDto">Данные для пополнения</param>
    /// <returns>Результат операции</returns>
    [HttpPost("deposit")]
    public async Task<IActionResult> DepositMoney([FromBody] DepositDto depositDto)
    {
        var operation = Mapper.Map<Core.Models.DepositOperation>(depositDto);
        var result = await _depositService.ExecuteAsync(operation);
        
        if (!result.Success)
            return BadRequest(new { error = result.Error });
            
        return Ok(Mapper.Map<MoneyOperationResultDto>(result));
    }
    
    /// <summary>
    /// Выполняет конвертацию валюты пользователя
    /// </summary>
    /// <param name="conversionDto">Данные для конвертации</param>
    /// <returns>Результат операции</returns>
    [HttpPost("convert")]
    public async Task<IActionResult> ConvertMoney([FromBody] ConversionDto conversionDto)
    {
        var operation = Mapper.Map<Core.Models.ConversionOperation>(conversionDto);
        var result = await _conversionService.ExecuteAsync(operation);
        
        if (!result.Success)
            return BadRequest(new { error = result.Error });
            
        return Ok(Mapper.Map<MoneyOperationResultDto>(result));
    }
    
    /// <summary>
    /// Выдает приветственный бонус новому пользователю
    /// </summary>
    /// <param name="welcomeBonusDto">Данные пользователя</param>
    /// <returns>Результат операции</returns>
    [HttpPost("welcomeBonus")]
    public async Task<IActionResult> GrantWelcomeBonus([FromBody] WelcomeBonusDto welcomeBonusDto)
    {
        var operation = Mapper.Map<Core.Models.WelcomeBonusOperation>(welcomeBonusDto);
        var result = await _welcomeBonusService.ExecuteAsync(operation);
        
        if (!result.Success)
            return BadRequest(new { error = result.Error });
            
        return Ok(Mapper.Map<MoneyOperationResultDto>(result));
    }
} 