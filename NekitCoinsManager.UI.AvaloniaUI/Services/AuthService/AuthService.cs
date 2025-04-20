using System;
using System.Threading.Tasks;
using NekitCoinsManager.Core.Services;
using NekitCoinsManager.Models;
using NekitCoinsManager.Shared.HttpClient;

namespace NekitCoinsManager.Services;

public class AuthService : IAuthService
{
    private readonly IUserServiceClient _userServiceClient;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthTokenServiceClient _authTokenServiceClient;
    private readonly IHardwareInfoService _hardwareInfoService;
    private readonly IUserSettingsService _userSettingsService;
    private readonly IAppSettingsService _appSettingsService;

    public AuthService(
        IUserServiceClient userServiceClient,
        IAuthTokenServiceClient authTokenServiceClient,
        ICurrentUserService currentUserService,
        IHardwareInfoService hardwareInfoService,
        IUserSettingsService userSettingsService,
        IAppSettingsService appSettingsService)
    {
        _userServiceClient = userServiceClient;
        _currentUserService = currentUserService;
        _authTokenServiceClient = authTokenServiceClient;
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

        // Аутентифицируем пользователя по логшну + паролю
        var authResult = await _userServiceClient.AuthenticateUserAsync(username, password);
        if (!authResult.success || authResult.user == null)
        {
            return (false, authResult.error ?? "Ошибка аутентификации");
        }

        // Получаем ID железа
        var hardwareId = await _hardwareInfoService.GetHardwareIdAsync();

        // Создаем токен авторизации
        var tokenDto = await _authTokenServiceClient.CreateTokenAsync(authResult.user.Id, hardwareId);

        // Сохраняем токен в настройках пользователя
        var settings = new UserSettings { AuthToken = tokenDto.Token };
        await _userSettingsService.SaveSettingsAsync(authResult.user.Id, settings);

        // Сохраняем ID пользователя в настройках приложения
        _appSettingsService.Settings.LoggedInUserId = authResult.user.Id;
        await _appSettingsService.SaveSettings();

        _currentUserService.SetCurrentUser(authResult.user);
        return (true, tokenDto.Token);
    }

    public async Task<(bool success, string? error)> RestoreSessionAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return (false, "Токен не может быть пустым");
        }

        var hardwareId = await _hardwareInfoService.GetHardwareIdAsync();
        var sessionResult = await _authTokenServiceClient.RestoreSessionAsync(token, hardwareId);

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
        if (currentUser != null)
        {
            await _authTokenServiceClient.DeactivateAllUserTokensAsync(currentUser.Id);
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
            return result; // Возвращаем сообщение об ошибке из RestoreSessionAsync
        }
        
        return (true, null);
    }
} 