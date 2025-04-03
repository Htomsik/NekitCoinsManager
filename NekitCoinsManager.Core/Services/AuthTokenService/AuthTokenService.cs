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

        // Валидируем новый токен перед созданием
        var (isValid, validationError) = await _tokenRepository.ValidateCreateAsync(token);
        if (!isValid)
        {
            // В случае ошибки валидации генерируем новый токен
            // (это маловероятно, но может произойти в случае коллизии токенов)
            if (validationError == ErrorCode.AuthTokenAlreadyExists)
            {
                token.Token = GenerateSecureToken();
                // Повторная проверка
                (isValid, validationError) = await _tokenRepository.ValidateCreateAsync(token);
                if (!isValid)
                {
                    throw new InvalidOperationException($"Не удалось создать токен: {validationError}");
                }
            }
            else
            {
                throw new InvalidOperationException($"Не удалось создать токен: {validationError}");
            }
        }

        await _tokenRepository.AddAsync(token);
        return token;
    }

    public async Task<UserAuthToken?> ValidateTokenAsync(string token, string hardwareId)
    {
        // Используем новый метод валидации из репозитория
        var (isValid, validationError) = await _tokenRepository.ValidateTokenAsync(token, hardwareId);
        if (!isValid)
        {
            // Если токен невалидный и это из-за истечения срока, обновим его статус
            if (validationError == ErrorCode.AuthTokenExpired)
            {
                var authToken = await _tokenRepository.GetByTokenAsync(token);
                if (authToken != null)
                {
                    authToken.IsActive = false;
                    await _tokenRepository.UpdateAsync(authToken);
                }
            }
            return null;
        }

        // Токен валидный, возвращаем его
        return await _tokenRepository.GetByTokenAsync(token);
    }

    public async Task DeactivateTokenAsync(int tokenId)
    {
        var token = await _tokenRepository.GetByIdAsync(tokenId);
        if (token != null)
        {
            token.IsActive = false;
            
            // Валидируем обновление
            var (isValid, validationError) = await _tokenRepository.ValidateUpdateAsync(token);
            if (!isValid)
            {
                throw new InvalidOperationException($"Не удалось деактивировать токен: {validationError}");
            }
            
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