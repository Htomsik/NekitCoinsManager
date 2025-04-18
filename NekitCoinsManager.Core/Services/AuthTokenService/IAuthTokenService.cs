using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Core.Services;

public interface IAuthTokenService
{
    Task<UserAuthToken> CreateTokenAsync(int userId, string hardwareId);
    Task<UserAuthToken?> ValidateTokenAsync(string token, string hardwareId);
    Task DeactivateTokenAsync(int tokenId);
    Task DeactivateAllUserTokensAsync(int userId);
} 