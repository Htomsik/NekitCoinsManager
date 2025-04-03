using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Core.Repositories;

public interface IUserAuthTokenRepository : IRepository<UserAuthToken>
{
    Task<UserAuthToken?> GetByTokenAsync(string token);
    Task<IEnumerable<UserAuthToken>> GetUserTokensAsync(int userId);
    Task<bool> IsTokenValidAsync(string token, string hardwareId);
    Task DeactivateAllUserTokensAsync(int userId);
    Task<bool> HasActiveTokensAsync(int userId);
    
    // Специальный метод валидации токена
    Task<(bool isValid, ErrorCode? error)> ValidateTokenAsync(string token, string hardwareId);
} 