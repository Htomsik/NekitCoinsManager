using System.Threading.Tasks;
using NekitCoinsManager.Models;
using NekitCoinsManager.Shared.DTO;
using NekitCoinsManager.Shared.HttpClient;

namespace NekitCoinsManager.Services;

public class AuthService : IAuthService
{
    private readonly IUserAuthServiceClient _userAuthServiceClient;
    private readonly ICurrentUserService _currentUserService;
    private readonly IHardwareInfoService _hardwareInfoService;
    private readonly IUserSettingsService _userSettingsService;
    private readonly IAppSettingsService _appSettingsService;

    public AuthService(
        IUserAuthServiceClient userAuthServiceClient,
        ICurrentUserService currentUserService,
        IHardwareInfoService hardwareInfoService,
        IUserSettingsService userSettingsService,
        IAppSettingsService appSettingsService)
    {
        _userAuthServiceClient = userAuthServiceClient;
        _currentUserService = currentUserService;
        _hardwareInfoService = hardwareInfoService;
        _userSettingsService = userSettingsService;
        _appSettingsService = appSettingsService;
    }

    public async Task<(bool success, string? error)> LoginAsync(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return (false, "Введите имя пользователя");
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            return (false, "Введите пароль");
        }

        // Получаем ID железа
        var hardwareId = await _hardwareInfoService.GetHardwareIdAsync();

        var loginDto = new UserAuthLoginDto
        {
            Username = username,
            Password = password,
            HardwareId = hardwareId
        };

        // Аутентифицируем пользователя и получаем токен в одном запросе
        var authResult = await _userAuthServiceClient.AuthenticateUserAsync(loginDto);
        if (!authResult.success || authResult.user == null)
        {
            return (false, authResult.error ?? "Ошибка аутентификации");
        }

        // Проверяем, что токен был успешно создан
        if (authResult.token == null)
        {
            return (false, "Не удалось создать токен авторизации");
        }

        // Сохраняем токен и id машины в настройках пользователя
        var settings = new UserSettings { AuthToken = authResult.token.Token, HardwareId = hardwareId };
        await _userSettingsService.SaveSettingsAsync(authResult.user.Id, settings);

        // Сохраняем ID пользователя в настройках приложения
        _appSettingsService.Settings.LoggedInUserId = authResult.user.Id;
        await _appSettingsService.SaveSettings();

        _currentUserService.SetCurrentUser(authResult.user);
        return (true, authResult.token.Token);
    }

    private async Task<(bool success, string? error)> RestoreSessionAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return (false, "Токен не может быть пустым");
        }

        var hardwareId = await _hardwareInfoService.GetHardwareIdAsync();
        
        var validateDto = new UserAuthTokenValidateDto
        {
            Token = token,
            HardwareId = hardwareId
        };
        
        // Используем IUserAuthServiceClient для восстановления сессии
        var sessionResult = await _userAuthServiceClient.RestoreSessionAsync(validateDto);

        if (!sessionResult.success || sessionResult.user == null)
        {
            return (false, sessionResult.error ?? "Ошибка восстановления сессии");
        }

        // Обновляем ID пользователя в настройках приложения
        _appSettingsService.Settings.LoggedInUserId = sessionResult.user.Id;
        await _appSettingsService.SaveSettings();

        _currentUserService.SetCurrentUser(sessionResult.user);
        return (true, null);
    }

    public async Task<(bool success, string? error)> LogoutAsync()
    {
        var currentUser = _currentUserService.CurrentUser;
        if (currentUser == null)
        {
            return (false, "Пользователь не авторизован");
        }

        var hardwareId = await _hardwareInfoService.GetHardwareIdAsync();
        
        var logoutDto = new UserAuthLogoutDto
        {
            UserId = currentUser.Id,
            HardwareId = hardwareId
        };
        
        // Выполняем выход через клиент
        var logoutResult = await _userAuthServiceClient.LogoutAsync(logoutDto);
        if (!logoutResult.success)
        {
            return logoutResult;
        }

        // Очищаем ID пользователя в настройках приложения
        _appSettingsService.Settings.LoggedInUserId = 0;
        await _appSettingsService.SaveSettings();

        _currentUserService.SetCurrentUser(null);
        return (true, null);
    }

    public async Task<(bool success, string? error)> TryRestoreSessionAsync()
    {
        // Загружаем настройки приложения для обеспечения актуальности данных
        await _appSettingsService.LoadSettings();
        
        // Проверяем наличие ID пользователя в настройках приложения
        int userId = _appSettingsService.Settings.LoggedInUserId;
        if (userId <= 0)
        {
            return (false, "Отсутствует сохраненная сессия");
        }
        
        // Загружаем настройки пользователя, чтобы получить токен
        var userSettings = await _userSettingsService.LoadSettingsAsync(userId);
        if (userSettings?.AuthToken == null)
        {
            // Токен не найден, сбрасываем настройки приложения
            _appSettingsService.Settings.LoggedInUserId = 0;
            await _appSettingsService.SaveSettings();
            return (false, "Токен авторизации не найден");
        }
        
        // Восстанавливаем сессию с использованием токена
        var result = await RestoreSessionAsync(userSettings.AuthToken);
        
        // Если восстановление не удалось, сбрасываем настройки приложения
        if (!result.success)
        {
            _appSettingsService.Settings.LoggedInUserId = 0;
            await _appSettingsService.SaveSettings();
            return result; // Возвращаем сообщение об ошибки из RestoreSessionAsync
        }
        
        return (true, null);
    }
}