using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Core.Services;

/// <summary>
/// Сервис для работы с пользователями (CRUD)
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Получает список всех пользователей
    /// </summary>
    Task<IEnumerable<User>> GetUsersAsync();
    
    /// <summary>
    /// Получает пользователя по имени
    /// </summary>
    Task<User?> GetUserByUsernameAsync(string username);
    
    /// <summary>
    /// Получает пользователя по идентификатору
    /// </summary>
    Task<User?> GetUserByIdAsync(int userId);
    
    /// <summary>
    /// Удаляет пользователя по идентификатору
    /// </summary>
    Task<(bool success, string? error)> DeleteUserAsync(int userId);
} 