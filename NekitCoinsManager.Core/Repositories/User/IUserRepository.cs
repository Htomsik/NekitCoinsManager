using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Core.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);
    
    Task<bool> IsUsernameUniqueAsync(string username, int? excludeId = null);
    
    Task<User?> GetBankAccountAsync();
    
    Task<(bool isValid, ErrorCode? error)> ValidatePasswordAsync(string password);
} 