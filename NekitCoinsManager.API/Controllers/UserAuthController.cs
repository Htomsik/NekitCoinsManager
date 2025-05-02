using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using NekitCoinsManager.Core.Services;
using NekitCoinsManager.Shared.Constants;
using NekitCoinsManager.Shared.DTO;

namespace NekitCoinsManager.API.Controllers;

/// <summary>
/// Контроллер для аутентификации и регистрации пользователей
/// </summary>
public class UserAuthController : BaseApiController
{
    private readonly IUserAuthService _userAuthService;

    /// <summary>
    /// Конструктор контроллера
    /// </summary>
    public UserAuthController(
        IUserAuthService userAuthService,
        IMapper mapper) : base(mapper)
    {
        _userAuthService = userAuthService;
    }

    /// <summary>
    /// Проверяет пароль пользователя
    /// </summary>
    /// <param name="request">Данные для проверки пароля</param>
    /// <returns>Результат проверки</returns>
    [HttpPost("verifyPassword")]
    public async Task<ActionResult<object>> VerifyPassword([FromBody] UserAuthLoginDto request)
    {
        var (success, error) = await _userAuthService.VerifyPasswordAsync(request.Username, request.Password);
        
        if (!success)
            return BadRequest(new { error });
            
        return Ok(new { success });
    }

    /// <summary>
    /// Аутентифицирует пользователя и возвращает его данные при успешной аутентификации
    /// </summary>
    /// <param name="request">Данные для аутентификации</param>
    /// <returns>Данные пользователя и токен аутентификации</returns>
    [HttpPost("authenticate")]
    public async Task<ActionResult<object>> AuthenticateUser([FromBody] UserAuthLoginDto request)
    {
        var (success, error, user, token) = await _userAuthService.AuthenticateUserAsync(
            request.Username, request.Password, request.HardwareId);
            
        if (!success || user == null || token == null)
            return BadRequest(new { error });
            
        return Ok(new { 
            user = Mapper.Map<UserDto>(user),
            token = Mapper.Map<UserAuthTokenDto>(token)
        });
    }

    /// <summary>
    /// Регистрирует нового пользователя
    /// </summary>
    /// <param name="request">Данные для регистрации</param>
    /// <returns>Результат регистрации</returns>
    [HttpPost("register")]
    public async Task<ActionResult<object>> RegisterUser([FromBody] UserAuthRegistrationDto request)
    {
        var (success, error) = await _userAuthService.RegisterUserAsync(
            request.Username, request.Password, request.ConfirmPassword);
            
        if (!success)
            return BadRequest(new { error });
            
        return Ok(new { success, message = "Пользователь успешно зарегистрирован" });
    }

    /// <summary>
    /// Восстанавливает сессию пользователя по токену аутентификации
    /// </summary>
    /// <param name="request">Данные для восстановления сессии</param>
    /// <returns>Данные пользователя при успешном восстановлении</returns>
    [HttpPost("restoreSession")]
    public async Task<ActionResult<object>> RestoreSession([FromBody] UserAuthTokenValidateDto request)
    {
        // Удаляем префикс Bearer если он есть
        var token = request.Token;
        if (token.StartsWith(HttpHeaderNames.BearerPrefix, StringComparison.OrdinalIgnoreCase))
            token = token.Substring(HttpHeaderNames.BearerPrefix.Length);
            
        var (success, error, user) = await _userAuthService.RestoreSessionAsync(token, request.HardwareId);
            
        if (!success || user == null)
            return BadRequest(new { error });
            
        return Ok(Mapper.Map<UserDto>(user));
    }

    /// <summary>
    /// Выполняет выход пользователя из системы, деактивируя токен
    /// </summary>
    /// <param name="request">Данные для выхода из системы</param>
    /// <returns>Результат операции</returns>
    [HttpPost("logout")]
    public async Task<ActionResult<object>> Logout([FromBody] UserAuthLogoutDto request)
    {
        var (success, error) = await _userAuthService.LogoutAsync(request.UserId, request.HardwareId);
            
        if (!success)
            return BadRequest(new { error });
            
        return Ok(new { success, message = "Выход выполнен успешно" });
    }
}
