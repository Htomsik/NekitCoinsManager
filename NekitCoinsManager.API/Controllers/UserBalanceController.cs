using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using NekitCoinsManager.Core.Services;
using NekitCoinsManager.Shared.DTO;

namespace NekitCoinsManager.API.Controllers;

/// <summary>
/// Контроллер для управления балансами пользователей
/// </summary>
public class UserBalanceController : BaseApiController
{
    private readonly IUserBalanceService _userBalanceService;

    /// <summary>
    /// Конструктор контроллера
    /// </summary>
    public UserBalanceController(
        IUserBalanceService userBalanceService,
        IMapper mapper) : base(mapper)
    {
        _userBalanceService = userBalanceService;
    }

    /// <summary>
    /// Получает все балансы пользователя
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <returns>Список балансов пользователя</returns>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<UserBalanceDto>>> GetUserBalances(int userId)
    {
        var balances = await _userBalanceService.GetUserBalancesAsync(userId);
        return Ok(Mapper.Map<List<UserBalanceDto>>(balances));
    }

    /// <summary>
    /// Получает баланс пользователя по указанной валюте
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="currencyId">Идентификатор валюты</param>
    /// <returns>Баланс пользователя</returns>
    [HttpGet("user/{userId}/currency/{currencyId}")]
    public async Task<ActionResult<UserBalanceDto>> GetUserBalance(int userId, int currencyId)
    {
        var balance = await _userBalanceService.GetUserBalanceAsync(userId, currencyId);
        if (balance == null)
            return NotFound(new { error = "Баланс не найден" });

        return Ok(Mapper.Map<UserBalanceDto>(balance));
    }

    /// <summary>
    /// Создает новый баланс для пользователя
    /// </summary>
    /// <param name="balanceDto">Данные для создания баланса</param>
    /// <returns>Результат операции</returns>
    [HttpPost]
    public async Task<IActionResult> CreateBalance([FromBody] UserBalanceModifyDto balanceDto)
    {
        var (success, error) = await _userBalanceService.CreateBalanceAsync(
            balanceDto.UserId,
            balanceDto.CurrencyId,
            balanceDto.Amount);

        if (!success)
            return BadRequest(new { error });

        return Ok(new { message = "Баланс успешно создан" });
    }

    /// <summary>
    /// Обновляет баланс пользователя
    /// </summary>
    /// <param name="balanceDto">Данные для обновления баланса</param>
    /// <returns>Результат операции</returns>
    [HttpPut]
    public async Task<IActionResult> UpdateBalance([FromBody] UserBalanceModifyDto balanceDto)
    {
        var (success, error) = await _userBalanceService.UpdateBalanceAsync(
            balanceDto.UserId,
            balanceDto.CurrencyId,
            balanceDto.Amount);

        if (!success)
            return BadRequest(new { error });

        return Ok(new { message = "Баланс успешно обновлен" });
    }

    /// <summary>
    /// Переводит средства между пользователями
    /// </summary>
    /// <param name="transferDto">Данные для перевода</param>
    /// <returns>Результат операции</returns>
    [HttpPost("transfer")]
    public async Task<IActionResult> TransferBalance([FromBody] UserBalanceTransferDto transferDto)
    {
        var (success, error) = await _userBalanceService.TransferBalanceAsync(
            transferDto.UserId,
            transferDto.ToUserId,
            transferDto.CurrencyId,
            transferDto.Amount);

        if (!success)
            return BadRequest(new { error });

        return Ok(new { message = "Перевод успешно выполнен" });
    }

    /// <summary>
    /// Получает баланс пользователя по указанной валюте или создает новый, если не существует
    /// </summary>
    /// <param name="balanceDto">Данные для получения или создания баланса</param>
    /// <returns>Баланс пользователя</returns>
    [HttpPost("getOrCreate")]
    public async Task<ActionResult<UserBalanceDto>> GetOrCreateBalance([FromBody] UserBalanceModifyDto balanceDto)
    {
        var (success, error, balance) = await _userBalanceService.GetOrCreateBalanceAsync(
            balanceDto.UserId,
            balanceDto.CurrencyId,
            balanceDto.Amount);

        if (!success)
            return BadRequest(new { error });

        return Ok(Mapper.Map<UserBalanceDto>(balance));
    }

    /// <summary>
    /// Проверяет, доступна ли указанная сумма на балансе пользователя
    /// </summary>
    /// <param name="balanceDto">Данные для проверки баланса</param>
    /// <returns>Результат проверки</returns>
    [HttpPost("validate")]
    public async Task<ActionResult<bool>> ValidateUserBalance([FromBody] UserBalanceModifyDto balanceDto)
    {
        var (isValid, errorMessage) = await _userBalanceService.ValidateUserBalanceAsync(
            balanceDto.UserId, 
            balanceDto.CurrencyId, 
            balanceDto.Amount);
            
        if (!isValid)
            return Ok(new { isValid, errorMessage });

        return Ok(new { isValid, errorMessage = (string?)null });
    }
} 