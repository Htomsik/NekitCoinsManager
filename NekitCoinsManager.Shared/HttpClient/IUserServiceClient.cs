using System.Collections.Generic;
using System.Threading.Tasks;
using NekitCoinsManager.Shared.DTO;
using NekitCoinsManager.Shared.DTO.Operations;

namespace NekitCoinsManager.Shared.HttpClient;

/// <summary>
/// Клиентский интерфейс для работы с пользователями
/// </summary>
public interface IUserServiceClient
{
    /// <summary>
    /// Получает список всех пользователей
    /// </summary>
    /// <returns>Список пользователей</returns>
    Task<IEnumerable<UserDto>> GetUsersAsync();
    
    /// <summary>
    /// Получает пользователя по имени пользователя
    /// </summary>
    /// <param name="username">Имя пользователя</param>
    /// <returns>Пользователь или null, если не найден</returns>
    Task<UserDto?> GetUserByUsernameAsync(string username);
    
    /// <summary>
    /// Получает пользователя по идентификатору
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <returns>Пользователь или null, если не найден</returns>
    Task<UserDto?> GetUserByIdAsync(int userId);
    
    /// <summary>
    /// Удаляет пользователя
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <returns>Результат операции</returns>
    Task<(bool success, string? error)> DeleteUserAsync(int userId);
} 