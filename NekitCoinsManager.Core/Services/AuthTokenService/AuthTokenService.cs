using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using NekitCoinsManager.Core.Data;
using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Core.Services;

public class AuthTokenService : IAuthTokenService
{
    private readonly AppDbContext _dbContext;
    private const int TokenLength = 64;
    private const int TokenExpirationDays = 30;

    public AuthTokenService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserAuthToken> CreateTokenAsync(int userId, string hardwareId)
    {
        // Деактивируем все предыдущие токены пользователя
        await DeactivateAllUserTokensAsync(userId);

        var token = new UserAuthToken
        {
            UserId = userId,
            Token = GenerateSecureToken(),
            HardwareId = hardwareId,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(TokenExpirationDays),
            IsActive = true
        };

        _dbContext.AuthTokens.Add(token);
        await _dbContext.SaveChangesAsync();

        return token;
    }

    public async Task<UserAuthToken?> ValidateTokenAsync(string token, string hardwareId)
    {
        var authToken = await _dbContext.AuthTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == token && t.IsActive);

        if (authToken == null)
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
            await _dbContext.SaveChangesAsync();
            return null;
        }

        return authToken;
    }

    public async Task DeactivateTokenAsync(int tokenId)
    {
        var token = await _dbContext.AuthTokens.FindAsync(tokenId);
        if (token != null)
        {
            token.IsActive = false;
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task DeactivateAllUserTokensAsync(int userId)
    {
        var tokens = await _dbContext.AuthTokens
            .Where(t => t.UserId == userId && t.IsActive)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.IsActive = false;
        }

        await _dbContext.SaveChangesAsync();
    }

    private string GenerateSecureToken()
    {
        using var rng = new RNGCryptoServiceProvider();
        var bytes = new byte[TokenLength];
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }
} 