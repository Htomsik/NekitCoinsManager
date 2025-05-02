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
    /// <param name="request">Данные для проверки пароля</param>
    /// <returns>Результат операции</returns>
    Task<(bool success, string? error)> VerifyPasswordAsync(UserAuthLoginDto request);

    /// <summary>
    /// Аутентифицирует пользователя и возвращает его данные при успешной аутентификации
    /// </summary>
    /// <param name="request">Данные для аутентификации</param>
    /// <returns>Результат операции, пользователь и токен аутентификации при успехе</returns>
    Task<(bool success, string? error, UserDto? user, UserAuthTokenDto? token)> AuthenticateUserAsync(
        UserAuthLoginDto request);

    /// <summary>
    /// Регистрирует нового пользователя
    /// </summary>
    /// <param name="request">Данные для регистрации</param>
    /// <returns>Результат операции</returns>
    Task<(bool success, string? error)> RegisterUserAsync(UserAuthRegistrationDto request);

    /// <summary>
    /// Восстанавливает сессию пользователя по токену аутентификации
    /// </summary>
    /// <param name="request">Данные для восстановления сессии</param>
    /// <returns>Результат операции, пользователь при успехе</returns>
    Task<(bool success, string? error, UserDto? user)> RestoreSessionAsync(UserAuthTokenValidateDto request);

    /// <summary>
    /// Выполняет выход пользователя из системы, деактивируя токен
    /// </summary>
    /// <param name="request">Данные для выхода из системы</param>
    /// <returns>Результат операции</returns>
    Task<(bool success, string? error)> LogoutAsync(UserAuthLogoutDto request);
}