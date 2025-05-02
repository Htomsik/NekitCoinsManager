using System.Collections.Generic;
using System.Threading.Tasks;
using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Core.Services;

/// <summary>
/// Сервис для управления токенами аутентификации
/// </summary>
public interface IAuthTokenService
{
    /// <summary>
    /// Создает новый токен для пользователя
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="hardwareId">Идентификатор устройства</param>
    /// <returns>Созданный токен</returns>
    Task<UserAuthToken> CreateTokenAsync(int userId, string hardwareId);
    
    /// <summary>
    /// Проверяет валидность токена
    /// </summary>
    /// <param name="token">Токен для проверки</param>
    /// <param name="hardwareId">Идентификатор устройства</param>
    /// <returns>Токен, если он валидный, иначе null</returns>
    Task<UserAuthToken?> ValidateTokenAsync(string token, string hardwareId);
    
    /// <summary>
    /// Деактивирует указанный токен
    /// </summary>
    /// <param name="tokenId">Идентификатор токена</param>
    Task DeactivateTokenAsync(int tokenId);
    
    /// <summary>
    /// Деактивирует все токены пользователя
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    Task DeactivateAllUserTokensAsync(int userId);
    
    /// <summary>
    /// Деактивирует все токены пользователя на конкретном устройстве
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// /// <param name="hardwareId">Идентификатор устройства</param>
    Task DeactivateAllUserTokensAsync(int userId, string hardwareId);
    
    /// <summary>
    /// Получает все токены пользователя
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <returns>Список токенов</returns>
    Task<IEnumerable<UserAuthToken>> GetUserTokensAsync(int userId);
} 