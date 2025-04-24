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
    public async Task<(bool success, string? error)> VerifyPasswordAsync(UserAuthLoginDto request)
    {
        return await _userAuthService.VerifyPasswordAsync(request.Username, request.Password);
    }

    /// <inheritdoc />
    public async Task<(bool success, string? error, UserDto? user, UserAuthTokenDto? token)> AuthenticateUserAsync(UserAuthLoginDto request)
    {
        var result = await _userAuthService.AuthenticateUserAsync(request.Username, request.Password, request.HardwareId);
        
        var userDto = result.user != null ? _mapper.Map<UserDto>(result.user) : null;
        var tokenDto = result.token != null ? _mapper.Map<UserAuthTokenDto>(result.token) : null;
        
        return (result.success, result.error, userDto, tokenDto);
    }

    /// <inheritdoc />
    public async Task<(bool success, string? error)> RegisterUserAsync(UserAuthRegistrationDto request)
    {
        return await _userAuthService.RegisterUserAsync(request.Username, request.Password, request.ConfirmPassword);
    }
    
    /// <inheritdoc />
    public async Task<(bool success, string? error, UserDto? user)> RestoreSessionAsync(UserAuthTokenValidateDto request)
    {
        var result = await _userAuthService.RestoreSessionAsync(request.Token, request.HardwareId);
        
        var userDto = result.user != null ? _mapper.Map<UserDto>(result.user) : null;
        
        return (result.success, result.error, userDto);
    }

    /// <inheritdoc />
    public async Task<(bool success, string? error)> LogoutAsync(UserAuthLogoutDto request)
    {
        return await _userAuthService.LogoutAsync(request.UserId, request.HardwareId);
    }
}