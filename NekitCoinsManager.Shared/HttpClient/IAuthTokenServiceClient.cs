using NekitCoinsManager.Shared.DTO;

namespace NekitCoinsManager.Shared.HttpClient;

/// <summary>
/// Клиентский интерфейс для работы с токенами авторизации
/// </summary>
public interface IAuthTokenServiceClient
{
    /// <summary>
    /// Создает новый токен авторизации для пользователя
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <param name="hardwareId">ID оборудования</param>
    /// <returns>Созданный токен</returns>
    Task<UserAuthTokenDto> CreateTokenAsync(int userId, string hardwareId);
    
    /// <summary>
    /// Проверяет валидность токена
    /// </summary>
    /// <param name="token">Значение токена</param>
    /// <param name="hardwareId">ID оборудования</param>
    /// <returns>Токен, если он валидный, иначе null</returns>
    Task<UserAuthTokenDto?> ValidateTokenAsync(string token, string hardwareId);
    
    /// <summary>
    /// Деактивирует указанный токен
    /// </summary>
    /// <param name="tokenId">ID токена</param>
    Task DeactivateTokenAsync(int tokenId);
    
    /// <summary>
    /// Деактивирует все токены пользователя
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    Task DeactivateAllUserTokensAsync(int userId);
    
    /// <summary>
    /// Получает все токены пользователя
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <returns>Список токенов</returns>
    Task<IEnumerable<UserAuthTokenDto>> GetUserTokensAsync(int userId);
    
    /// <summary>
    /// Восстанавливает сессию пользователя по токену
    /// </summary>
    /// <param name="token">Значение токена</param>
    /// <param name="hardwareId">ID оборудования</param>
    /// <returns>Результат операции и данные пользователя при успехе</returns>
    Task<(bool success, string? error, UserDto? user)> RestoreSessionAsync(string token, string hardwareId);
} 