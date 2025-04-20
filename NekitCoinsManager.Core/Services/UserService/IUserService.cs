using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Core.Services;

public interface IUserService
{
    Task<IEnumerable<User>> GetUsersAsync();
    Task<User?> GetUserByUsernameAsync(string username);
    Task<User?> GetUserByIdAsync(int userId);
    Task<(bool success, string? error)> AddUserAsync(string username, string password, string confirmPassword);
    Task<(bool success, string? error)> DeleteUserAsync(int userId);
    Task<(bool success, string? error)> VerifyPasswordAsync(string username, string password);
    Task<(bool success, string? error, User? user)> AuthenticateUserAsync(string username, string password);
} 