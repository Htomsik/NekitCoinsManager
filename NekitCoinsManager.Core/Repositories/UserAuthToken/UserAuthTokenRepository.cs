using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NekitCoinsManager.Core.Data;
using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Core.Repositories;

public class UserAuthTokenRepository : BaseRepository<UserAuthToken>, IUserAuthTokenRepository
{
    public UserAuthTokenRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<UserAuthToken?> GetByTokenAsync(string token)
    {
        return await DbSet
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == token);
    }

    public async Task<IEnumerable<UserAuthToken>> GetUserTokensAsync(int userId)
    {
        return await DbSet
            .Where(t => t.UserId == userId)
            .ToListAsync();
    }

    public async Task<bool> IsTokenValidAsync(string token, string hardwareId)
    {
        var (isValid, _) = await ValidateTokenAsync(token, hardwareId);
        return isValid;
    }

    public async Task DeactivateAllUserTokensAsync(int userId)
    {
        // Получаем все активные токены пользователя
        var tokens = await DbSet
            .Where(t => t.UserId == userId && t.IsActive)
            .ToListAsync();
        
        // Деактивируем все токены
        foreach (var token in tokens)
        {
            token.IsActive = false;
            token.ExpiresAt = DateTime.UtcNow; // Устанавливаем время истечения на текущее время
        }
        
        // Сохраняем изменения
        await DbContext.SaveChangesAsync();
    }

    public async Task<bool> HasActiveTokensAsync(int userId)
    {
        return await DbSet
            .AnyAsync(t => t.UserId == userId && t.IsActive && t.ExpiresAt > DateTime.UtcNow);
    }

    // Реализация методов валидации
    public override async Task<(bool isValid, ErrorCode? error)> ValidateEntityAsync(UserAuthToken entity)
    {
        // Проверка обязательных полей
        if (entity.UserId <= 0)
            return (false, ErrorCode.AuthTokenUserIdInvalid);

        if (string.IsNullOrWhiteSpace(entity.Token))
            return (false, ErrorCode.AuthTokenEmpty);

        if (string.IsNullOrWhiteSpace(entity.HardwareId))
            return (false, ErrorCode.AuthTokenHardwareIdEmpty);

        // Валидация дат
        if (entity.CreatedAt == default)
            return (false, ErrorCode.AuthTokenCreatedAtInvalid);

        if (entity.ExpiresAt <= entity.CreatedAt)
            return (false, ErrorCode.AuthTokenExpirationDateInvalid);

        return (true, null);
    }

    public override async Task<(bool isValid, ErrorCode? error)> ValidateCreateAsync(UserAuthToken entity)
    {
        // Базовая валидация сущности
        var baseValidation = await ValidateEntityAsync(entity);
        if (!baseValidation.isValid)
            return baseValidation;

        // Проверка на уникальность токена
        var existingToken = await GetByTokenAsync(entity.Token);
        if (existingToken != null)
            return (false, ErrorCode.AuthTokenAlreadyExists);

        return (true, null);
    }

    public override async Task<(bool isValid, ErrorCode? error)> ValidateUpdateAsync(UserAuthToken entity)
    {
        // Проверка существования
        var existingToken = await GetByIdAsync(entity.Id);
        if (existingToken == null)
            return (false, ErrorCode.AuthTokenNotFound);

        return (true, null);
    }

    public async Task<(bool isValid, ErrorCode? error)> ValidateTokenAsync(string token, string hardwareId)
    {
        if (string.IsNullOrWhiteSpace(token))
            return (false, ErrorCode.AuthTokenEmpty);

        if (string.IsNullOrWhiteSpace(hardwareId))
            return (false, ErrorCode.AuthTokenHardwareIdEmpty);

        var authToken = await GetByTokenAsync(token);
        if (authToken == null)
            return (false, ErrorCode.AuthTokenNotFound);

        if (!authToken.IsActive)
            return (false, ErrorCode.AuthTokenInactive);

        if (authToken.HardwareId != hardwareId)
            return (false, ErrorCode.AuthTokenHardwareIdMismatch);

        if (authToken.ExpiresAt < DateTime.UtcNow)
            return (false, ErrorCode.AuthTokenExpired);

        return (true, null);
    }

    public override async Task<(bool canDelete, ErrorCode? error)> ValidateDeleteAsync(int id)
    {
        // Базовая проверка существования
        var baseValidation = await base.ValidateDeleteAsync(id);
        if (!baseValidation.canDelete)
            return baseValidation;

        return (true, null);
    }
} 