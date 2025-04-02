using System.Security.Cryptography;
using NekitCoinsManager.Core.Models;
using NekitCoinsManager.Core.Repositories;

namespace NekitCoinsManager.Core.Services;

public class AuthTokenService : IAuthTokenService
{
    private readonly IUserAuthTokenRepository _tokenRepository;
    private const int TokenLength = 64;
    private const int TokenExpirationDays = 30;

    public AuthTokenService(IUserAuthTokenRepository tokenRepository)
    {
        _tokenRepository = tokenRepository;
    }

    public async Task<UserAuthToken> CreateTokenAsync(int userId, string hardwareId)
    {
        // Деактивируем все предыдущие токены пользователя
        await _tokenRepository.DeactivateAllUserTokensAsync(userId);

        var token = new UserAuthToken
        {
            UserId = userId,
            Token = GenerateSecureToken(),
            HardwareId = hardwareId,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(TokenExpirationDays),
            IsActive = true
        };

        await _tokenRepository.AddAsync(token);
        return token;
    }

    public async Task<UserAuthToken?> ValidateTokenAsync(string token, string hardwareId)
    {
        var authToken = await _tokenRepository.GetByTokenAsync(token);

        if (authToken == null || !authToken.IsActive)
        {
            return null;
        }

        if (authToken.HardwareId != hardwareId)
        {
            return null;
        }

        if (authToken.ExpiresAt < DateTime.UtcNow)
        {
            authToken.IsActive = false;
            await _tokenRepository.UpdateAsync(authToken);
            return null;
        }

        return authToken;
    }

    public async Task DeactivateTokenAsync(int tokenId)
    {
        var token = await _tokenRepository.GetByIdAsync(tokenId);
        if (token != null)
        {
            token.IsActive = false;
            await _tokenRepository.UpdateAsync(token);
        }
    }

    public async Task DeactivateAllUserTokensAsync(int userId)
    {
        await _tokenRepository.DeactivateAllUserTokensAsync(userId);
    }

    public async Task<IEnumerable<UserAuthToken>> GetUserTokensAsync(int userId)
    {
        return await _tokenRepository.GetUserTokensAsync(userId);
    }

    private string GenerateSecureToken()
    {
        using var rng = new RNGCryptoServiceProvider();
        var bytes = new byte[TokenLength];
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }
} 