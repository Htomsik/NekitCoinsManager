using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NekitCoinsManager.Core.Data;
using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Core.Repositories;

public class UserAuthTokenRepository : IUserAuthTokenRepository
{
    private readonly AppDbContext _dbContext;

    public UserAuthTokenRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<UserAuthToken>> GetAllAsync()
    {
        return await _dbContext.AuthTokens
            .ToListAsync();
    }

    public async Task<UserAuthToken?> GetByIdAsync(int id)
    {
        return await _dbContext.AuthTokens
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<UserAuthToken> AddAsync(UserAuthToken entity)
    {
        await _dbContext.AuthTokens.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(UserAuthToken entity)
    {
        _dbContext.Entry(entity).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(UserAuthToken entity)
    {
        _dbContext.AuthTokens.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<UserAuthToken>> FindAsync(Expression<Func<UserAuthToken, bool>> predicate)
    {
        return await _dbContext.AuthTokens
            .Where(predicate)
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(Expression<Func<UserAuthToken, bool>> predicate)
    {
        return await _dbContext.AuthTokens.AnyAsync(predicate);
    }

    public async Task<UserAuthToken?> GetByTokenAsync(string token)
    {
        return await _dbContext.AuthTokens
            .FirstOrDefaultAsync(t => t.Token == token);
    }

    public async Task<IEnumerable<UserAuthToken>> GetUserTokensAsync(int userId)
    {
        return await _dbContext.AuthTokens
            .Where(t => t.UserId == userId)
            .ToListAsync();
    }

    public async Task<bool> IsTokenValidAsync(string token, string hardwareId)
    {
        var authToken = await GetByTokenAsync(token);
        if (authToken == null)
            return false;

        if (authToken.HardwareId != hardwareId)
            return false;

        if (authToken.ExpiresAt < DateTime.UtcNow)
            return false;

        return authToken.IsActive;
    }

    public async Task DeactivateAllUserTokensAsync(int userId)
    {
        var tokens = await GetUserTokensAsync(userId);
        foreach (var token in tokens)
        {
            token.IsActive = false;
        }
        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> HasActiveTokensAsync(int userId)
    {
        return await _dbContext.AuthTokens
            .AnyAsync(t => t.UserId == userId && t.IsActive && t.ExpiresAt > DateTime.UtcNow);
    }
} 