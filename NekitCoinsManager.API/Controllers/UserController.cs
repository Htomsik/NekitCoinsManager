using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using NekitCoinsManager.Core.Services;
using NekitCoinsManager.Shared.DTO;

namespace NekitCoinsManager.API.Controllers;

/// <summary>
/// Контроллер для работы с пользователями
/// </summary>
public class UserController : BaseApiController
{
    private readonly IUserService _userService;

    /// <summary>
    /// Конструктор контроллера
    /// </summary>
    public UserController(
        IUserService userService,
        IMapper mapper) : base(mapper)
    {
        _userService = userService;
    }

    /// <summary>
    /// Получает список всех пользователей
    /// </summary>
    /// <returns>Список пользователей</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        var users = await _userService.GetUsersAsync();
        return Ok(Mapper.Map<List<UserDto>>(users));
    }

    /// <summary>
    /// Получает пользователя по имени пользователя
    /// </summary>
    /// <param name="username">Имя пользователя</param>
    /// <returns>Пользователь или ошибка, если не найден</returns>
    [HttpGet("byUsername/{username}")]
    public async Task<ActionResult<UserDto>> GetUserByUsername(string username)
    {
        var user = await _userService.GetUserByUsernameAsync(username);
        if (user == null)
            return NotFound(new { error = "Пользователь не найден" });

        return Ok(Mapper.Map<UserDto>(user));
    }

    /// <summary>
    /// Получает пользователя по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор пользователя</param>
    /// <returns>Пользователь или ошибка, если не найден</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUserById(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound(new { error = "Пользователь не найден" });

        return Ok(Mapper.Map<UserDto>(user));
    }

    /// <summary>
    /// Регистрирует нового пользователя
    /// </summary>
    /// <param name="request">Данные для регистрации</param>
    /// <returns>Результат операции</returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegistrationDto request)
    {
        var (success, error) = await _userService.AddUserAsync(
            request.Username, 
            request.Password, 
            request.ConfirmPassword);

        if (!success)
            return BadRequest(new { error });

        return Ok(new { message = "Пользователь успешно зарегистрирован" });
    }

    /// <summary>
    /// Удаляет пользователя
    /// </summary>
    /// <param name="id">Идентификатор пользователя</param>
    /// <returns>Результат операции</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var (success, error) = await _userService.DeleteUserAsync(id);
        if (!success)
            return BadRequest(new { error });

        return Ok(new { message = "Пользователь успешно удален" });
    }

    /// <summary>
    /// Проверяет пароль пользователя
    /// </summary>
    /// <param name="request">Данные для проверки</param>
    /// <returns>Результат проверки</returns>
    [HttpPost("verifyPassword")]
    public async Task<IActionResult> VerifyPassword([FromBody] UserLoginDto request)
    {
        var (success, error) = await _userService.VerifyPasswordAsync(request.Username, request.Password);
        if (!success)
            return BadRequest(new { error });

        return Ok(new { message = "Пароль верный" });
    }

    /// <summary>
    /// Аутентифицирует пользователя
    /// </summary>
    /// <param name="request">Данные для входа</param>
    /// <returns>Данные пользователя при успешной аутентификации</returns>
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login([FromBody] UserLoginDto request)
    {
        var (success, error, user) = await _userService.AuthenticateUserAsync(request.Username, request.Password);
        if (!success)
            return BadRequest(new { error });

        if (user == null)
            return NotFound(new { error = "Пользователь не найден" });

        return Ok(Mapper.Map<UserDto>(user));
    }
} 