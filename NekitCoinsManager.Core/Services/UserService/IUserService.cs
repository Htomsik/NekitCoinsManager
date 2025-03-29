using System.Collections.Generic;
using System.Threading.Tasks;
using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Core.Services;

public interface IUserObserver
{
    void OnUsersChanged();
}

public interface IUserService
{
    IEnumerable<User> GetUsers();
    User? GetUserByUsername(string username);
    User? GetCurrentUser();
    void SetCurrentUser(User? user);
    Task<(bool success, string? error)> AddUserAsync(string username, string password, string confirmPassword);
    Task<(bool success, string? error)> DeleteUserAsync(int userId);
    Task UpdateUserBalance(int userId, decimal newBalance);
    void Subscribe(IUserObserver observer);
} 