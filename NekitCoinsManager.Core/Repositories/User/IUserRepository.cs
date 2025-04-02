using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Core.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);
    Task<IEnumerable<User>> GetActiveUsersAsync();
    Task<bool> IsUsernameUniqueAsync(string username, int? excludeId = null);
    Task<bool> IsBankAccountAsync(int userId);
    Task<User?> GetBankAccountAsync();
} 