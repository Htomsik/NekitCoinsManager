using System.Collections.Generic;
using System.Threading.Tasks;
using MapsterMapper;
using NekitCoinsManager.Core.Services;
using NekitCoinsManager.Shared.DTO;
using NekitCoinsManager.Shared.HttpClient;

namespace NekitCoinsManager.HttpClients;

/// <summary>
/// Локальная реализация клиента для работы с пользователями
/// </summary>
public class UserServiceLocalClient : IUserServiceClient
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    /// <summary>
    /// Создает экземпляр локального клиента для работы с пользователями
    /// </summary>
    /// <param name="userService">Оригинальный сервис для работы с пользователями</param>
    /// <param name="mapper">Инструмент для маппинга моделей</param>
    public UserServiceLocalClient(IUserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<UserDto>> GetUsersAsync()
    {
        var users = await _userService.GetUsersAsync();
        return _mapper.Map<IEnumerable<UserDto>>(users);
    }

    /// <inheritdoc />
    public async Task<UserDto?> GetUserByUsernameAsync(string username)
    {
        var user = await _userService.GetUserByUsernameAsync(username);
        return user != null ? _mapper.Map<UserDto>(user) : null;
    }

    /// <inheritdoc />
    public async Task<UserDto?> GetUserByIdAsync(int userId)
    {
        var user = await _userService.GetUserByIdAsync(userId);
        return user != null ? _mapper.Map<UserDto>(user) : null;
    }

    /// <inheritdoc />
    public async Task<(bool success, string? error)> AddUserAsync(string username, string password, string confirmPassword)
    {
        return await _userService.AddUserAsync(username, password, confirmPassword);
    }

    /// <inheritdoc />
    public async Task<(bool success, string? error)> DeleteUserAsync(int userId)
    {
        return await _userService.DeleteUserAsync(userId);
    }

    /// <inheritdoc />
    public async Task<(bool success, string? error)> VerifyPasswordAsync(string username, string password)
    {
        return await _userService.VerifyPasswordAsync(username, password);
    }

    /// <inheritdoc />
    public async Task<(bool success, string? error, UserDto? user)> AuthenticateUserAsync(string username, string password)
    {
        var result = await _userService.AuthenticateUserAsync(username, password);
        var userDto = result.user != null ? _mapper.Map<UserDto>(result.user) : null;
        return (result.success, result.error, userDto);
    }
} 