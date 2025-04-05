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
    /// Добавляет нового пользователя
    /// </summary>
    /// <param name="username">Имя пользователя</param>
    /// <param name="password">Пароль</param>
    /// <param name="confirmPassword">Подтверждение пароля</param>
    /// <returns>Результат операции</returns>
    Task<OperationResultDto> AddUserAsync(string username, string password, string confirmPassword);
    
    /// <summary>
    /// Удаляет пользователя
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <returns>Результат операции</returns>
    Task<OperationResultDto> DeleteUserAsync(int userId);
    
    /// <summary>
    /// Проверяет пароль пользователя
    /// </summary>
    /// <param name="username">Имя пользователя</param>
    /// <param name="password">Пароль для проверки</param>
    /// <returns>Результат операции</returns>
    Task<OperationResultDto> VerifyPasswordAsync(string username, string password);
} 