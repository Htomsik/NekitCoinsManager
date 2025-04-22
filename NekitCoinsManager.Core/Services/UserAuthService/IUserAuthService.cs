using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Core.Services;

/// <summary>
/// Сервис для аутентификации, авторизации и регистрации пользователей
/// </summary>
public interface IUserAuthService
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
    Task<(bool success, string? error, User? user, UserAuthToken? token)> AuthenticateUserAsync(string username, string password, string hardwareId);
    
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
    Task<(bool success, string? error, User? user)> RestoreSessionAsync(string token, string hardwareId);
    
    /// <summary>
    /// Выполняет выход пользователя из системы, деактивируя все его токены
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="hardwareId">Идентификатор устройства</param>
    /// <returns>Результат операции</returns>
    Task<(bool success, string? error)> LogoutAsync(int userId, string hardwareId);
} 