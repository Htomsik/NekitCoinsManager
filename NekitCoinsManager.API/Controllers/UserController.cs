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
} 