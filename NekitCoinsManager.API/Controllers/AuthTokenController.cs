using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using NekitCoinsManager.Core.Services;
using NekitCoinsManager.Shared.DTO;

namespace NekitCoinsManager.API.Controllers;

/// <summary>
/// Контроллер для работы с токенами авторизации
/// </summary>
public class AuthTokenController : BaseApiController
{
    private readonly IAuthTokenService _authTokenService;

    /// <summary>
    /// Конструктор контроллера
    /// </summary>
    public AuthTokenController(
        IAuthTokenService authTokenService,
        IMapper mapper) : base(mapper)
    {
        _authTokenService = authTokenService;
    }

    /// <summary>
    /// Создает новый токен авторизации для пользователя
    /// </summary>
    /// <param name="request">Данные для создания токена</param>
    /// <returns>Созданный токен</returns>
    [HttpPost("create")]
    public async Task<ActionResult<UserAuthTokenDto>> CreateToken([FromBody] UserAuthTokenCreateDto request)
    {
        var result = await _authTokenService.CreateTokenAsync(request.UserId, request.HardwareId);
        if (result == null)
            return NotFound(new { error = "Не удалось создать токен. Пользователь не найден." });

        return Ok(Mapper.Map<UserAuthTokenDto>(result));
    }

    /// <summary>
    /// Проверяет валидность токена
    /// </summary>
    /// <param name="request">Данные для проверки токена</param>
    /// <returns>Токен, если он валидный, иначе null</returns>
    [HttpPost("validate")]
    public async Task<ActionResult<UserAuthTokenDto>> ValidateToken([FromBody] UserAuthTokenValidateDto request)
    {
        var result = await _authTokenService.ValidateTokenAsync(request.Token, request.HardwareId);
        if (result == null)
            return NotFound(new { error = "Токен недействителен или истек срок его действия." });

        return Ok(Mapper.Map<UserAuthTokenDto>(result));
    }

    /// <summary>
    /// Деактивирует указанный токен
    /// </summary>
    /// <param name="tokenId">ID токена</param>
    [HttpPost("deactivate/{tokenId}")]
    public async Task<IActionResult> DeactivateToken(int tokenId)
    {
        await _authTokenService.DeactivateTokenAsync(tokenId);
        return Ok(new { message = "Токен успешно деактивирован" });
    }

    /// <summary>
    /// Деактивирует все токены пользователя
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    [HttpPost("deactivateAll/{userId}")]
    public async Task<IActionResult> DeactivateAllUserTokens(int userId)
    {
        await _authTokenService.DeactivateAllUserTokensAsync(userId);
        return Ok(new { message = "Все токены пользователя деактивированы" });
    }

    /// <summary>
    /// Получает все токены пользователя
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <returns>Список токенов</returns>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<UserAuthTokenDto>>> GetUserTokens(int userId)
    {
        var tokens = await _authTokenService.GetUserTokensAsync(userId);
        return Ok(Mapper.Map<List<UserAuthTokenDto>>(tokens));
    }

    /// <summary>
    /// Восстанавливает сессию пользователя по токену
    /// </summary>
    /// <param name="request">Данные для восстановления сессии</param>
    /// <returns>Результат операции и данные пользователя при успехе</returns>
    [HttpPost("restoreSession")]
    public async Task<ActionResult<UserDto>> RestoreSession([FromBody] UserAuthTokenValidateDto request)
    {
        // Делегируем логику проверки токена и получения пользователя сервису AuthTokenService
        var (success, error, user) = await _authTokenService.RestoreSessionAsync(request.Token, request.HardwareId);
        
        if (!success)
            return BadRequest(new { error });
            
        if (user == null)
            return NotFound(new { error = "Пользователь не найден" });
            
        return Ok(Mapper.Map<UserDto>(user));
    }
} 