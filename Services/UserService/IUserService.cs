using System.Collections.Generic;
using System.Threading.Tasks;
using NekitCoinsManager.Models;

namespace NekitCoinsManager.Services;

public interface IUserObserver
{
    void OnUsersChanged();
}

public interface IUserService
{
    IEnumerable<User> GetUsers();
    Task<(bool success, string? error)> AddUserAsync(string username, string password, string confirmPassword);
    Task<(bool success, string? error)> DeleteUserAsync(int userId);
    Task UpdateUserBalance(int userId, decimal newBalance);
    void Subscribe(IUserObserver observer);
} 