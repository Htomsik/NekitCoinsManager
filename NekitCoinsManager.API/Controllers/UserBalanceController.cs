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
} 