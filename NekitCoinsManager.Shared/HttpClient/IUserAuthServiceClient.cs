using System.Threading.Tasks;
using NekitCoinsManager.Shared.DTO;

namespace NekitCoinsManager.Shared.HttpClient;

/// <summary>
/// Клиентский интерфейс для работы с аутентификацией и регистрацией пользователей
/// </summary>
public interface IUserAuthServiceClient
{
    /// <summary>
    /// Проверяет пароль пользователя
    /// </summary>
    /// <param name="username">Имя пользователя</param>
    /// <param name="password">Пароль для проверки</param>
    /// <returns>Результат операции</returns>
    Task<(bool success, string? error)> VerifyPasswordAsync(string username, string password);
    
    /// <summary>
    /// Аутентифицирует пользователя и возвращает его данные при успешной аутентификации
    /// </summary>
    /// <param name="username">Имя пользователя</param>
    /// <param name="password">Пароль</param>
    /// <param name="hardwareId">Идентификатор устройства для создания токена</param>
    /// <returns>Результат операции, пользователь и токен аутентификации при успехе</returns>
    Task<(bool success, string? error, UserDto? user, UserAuthTokenDto? token)> AuthenticateUserAsync(string username, string password, string hardwareId);
    
    /// <summary>
    /// Регистрирует нового пользователя
    /// </summary>
    /// <param name="username">Имя пользователя</param>
    /// <param name="password">Пароль</param>
    /// <param name="confirmPassword">Подтверждение пароля</param>
    /// <returns>Результат операции</returns>
    Task<(bool success, string? error)> RegisterUserAsync(string username, string password, string confirmPassword);
    
    /// <summary>
    /// Восстанавливает сессию пользователя по токену аутентификации
    /// </summary>
    /// <param name="token">Токен аутентификации</param>
    /// <param name="hardwareId">Идентификатор устройства</param>
    /// <returns>Результат операции, пользователь при успехе</returns>
    Task<(bool success, string? error, UserDto? user)> RestoreSessionAsync(string token, string hardwareId);
} 