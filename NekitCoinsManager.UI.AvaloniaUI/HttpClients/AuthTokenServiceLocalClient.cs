using System.Collections.Generic;
using System.Threading.Tasks;
using MapsterMapper;
using NekitCoinsManager.Core.Services;
using NekitCoinsManager.Shared.DTO;
using NekitCoinsManager.Shared.HttpClient;

namespace NekitCoinsManager.HttpClients;

/// <summary>
/// Локальная реализация клиента для работы с токенами авторизации
/// </summary>
public class AuthTokenServiceLocalClient : IAuthTokenServiceClient
{
    private readonly IAuthTokenService _authTokenService;
    private readonly IMapper _mapper;

    /// <summary>
    /// Создает экземпляр локального клиента для работы с токенами
    /// </summary>
    /// <param name="authTokenService">Оригинальный сервис для работы с токенами</param>
    /// <param name="mapper">Инструмент для маппинга моделей</param>
    public AuthTokenServiceLocalClient(IAuthTokenService authTokenService, IMapper mapper)
    {
        _authTokenService = authTokenService;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<UserAuthTokenDto> CreateTokenAsync(int userId, string hardwareId)
    {
        var token = await _authTokenService.CreateTokenAsync(userId, hardwareId);
        return _mapper.Map<UserAuthTokenDto>(token);
    }

    /// <inheritdoc />
    public async Task<UserAuthTokenDto?> ValidateTokenAsync(string token, string hardwareId)
    {
        var validToken = await _authTokenService.ValidateTokenAsync(token, hardwareId);
        return validToken != null ? _mapper.Map<UserAuthTokenDto>(validToken) : null;
    }

    /// <inheritdoc />
    public async Task DeactivateTokenAsync(int tokenId)
    {
        await _authTokenService.DeactivateTokenAsync(tokenId);
    }

    /// <inheritdoc />
    public async Task DeactivateAllUserTokensAsync(int userId)
    {
        await _authTokenService.DeactivateAllUserTokensAsync(userId);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<UserAuthTokenDto>> GetUserTokensAsync(int userId)
    {
        var tokens = await _authTokenService.GetUserTokensAsync(userId);
        return _mapper.Map<IEnumerable<UserAuthTokenDto>>(tokens);
    }
    
    /// <inheritdoc />
    public async Task<(bool success, string? error, UserDto? user)> RestoreSessionAsync(string token, string hardwareId)
    {
        var result = await _authTokenService.RestoreSessionAsync(token, hardwareId);
        var userDto = result.user != null ? _mapper.Map<UserDto>(result.user) : null;
        return (result.success, result.error, userDto);
    }
} 