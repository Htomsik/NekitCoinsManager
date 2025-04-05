using System.Collections.Generic;
using System.Threading.Tasks;
using MapsterMapper;
using NekitCoinsManager.Core.Services;
using NekitCoinsManager.Shared.DTO;
using NekitCoinsManager.Shared.DTO.Operations;
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
    public async Task<OperationResultDto> AddUserAsync(string username, string password, string confirmPassword)
    {
        var (success, error) = await _userService.AddUserAsync(username, password, confirmPassword);
        return success 
            ? OperationResultDto.CreateSuccess() 
            : OperationResultDto.CreateError(error ?? "Ошибка при создании пользователя");
    }

    /// <inheritdoc />
    public async Task<OperationResultDto> DeleteUserAsync(int userId)
    {
        var (success, error) = await _userService.DeleteUserAsync(userId);
        return success 
            ? OperationResultDto.CreateSuccess() 
            : OperationResultDto.CreateError(error ?? "Ошибка при удалении пользователя");
    }

    /// <inheritdoc />
    public async Task<OperationResultDto> VerifyPasswordAsync(string username, string password)
    {
        var (success, error) = await _userService.VerifyPasswordAsync(username, password);
        return success
            ? OperationResultDto.CreateSuccess()
            : OperationResultDto.CreateError(error ?? "Ошибка при проверке пароля");
    }
} 