using System.Threading.Tasks;
using MapsterMapper;
using NekitCoinsManager.Core.Services;
using NekitCoinsManager.Shared.DTO;
using NekitCoinsManager.Shared.HttpClient;

namespace NekitCoinsManager.HttpClients;

/// <summary>
/// Локальная реализация клиента для работы с аутентификацией и регистрацией пользователей
/// </summary>
public class UserAuthServiceLocalClient : IUserAuthServiceClient
{
    private readonly IUserAuthService _userAuthService;
    private readonly IMapper _mapper;

    /// <summary>
    /// Создает экземпляр локального клиента для работы с аутентификацией
    /// </summary>
    /// <param name="userAuthService">Оригинальный сервис для аутентификации пользователей</param>
    /// <param name="mapper">Инструмент для маппинга моделей</param>
    public UserAuthServiceLocalClient(IUserAuthService userAuthService, IMapper mapper)
    {
        _userAuthService = userAuthService;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<(bool success, string? error)> VerifyPasswordAsync(string username, string password)
    {
        return await _userAuthService.VerifyPasswordAsync(username, password);
    }

    /// <inheritdoc />
    public async Task<(bool success, string? error, UserDto? user, UserAuthTokenDto? token)> AuthenticateUserAsync(string username, string password, string hardwareId)
    {
        var result = await _userAuthService.AuthenticateUserAsync(username, password, hardwareId);
        
        var userDto = result.user != null ? _mapper.Map<UserDto>(result.user) : null;
        var tokenDto = result.token != null ? _mapper.Map<UserAuthTokenDto>(result.token) : null;
        
        return (result.success, result.error, userDto, tokenDto);
    }

    /// <inheritdoc />
    public async Task<(bool success, string? error)> RegisterUserAsync(string username, string password, string confirmPassword)
    {
        return await _userAuthService.RegisterUserAsync(username, password, confirmPassword);
    }
    
    /// <inheritdoc />
    public async Task<(bool success, string? error, UserDto? user)> RestoreSessionAsync(string token, string hardwareId)
    {
        var result = await _userAuthService.RestoreSessionAsync(token, hardwareId);
        
        var userDto = result.user != null ? _mapper.Map<UserDto>(result.user) : null;
        
        return (result.success, result.error, userDto);
    }
} 