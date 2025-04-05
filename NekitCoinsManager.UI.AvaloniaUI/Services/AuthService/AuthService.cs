using System.Threading.Tasks;
using NekitCoinsManager.Core.Models;
using NekitCoinsManager.Core.Services;
using NekitCoinsManager.Models;

namespace NekitCoinsManager.Services;

public class AuthService : IAuthService
{
    private readonly IUserService _userService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPasswordHasherService _passwordHasher;
    private readonly IAuthTokenService _authTokenService;
    private readonly IHardwareInfoService _hardwareInfoService;
    private readonly IUserSettingsService _userSettingsService;
    private readonly IAppSettingsService _appSettingsService;

    public AuthService(
        IUserService userService,
        ICurrentUserService currentUserService,
        IPasswordHasherService passwordHasher,
        IAuthTokenService authTokenService,
        IHardwareInfoService hardwareInfoService,
        IUserSettingsService userSettingsService,
        IAppSettingsService appSettingsService)
    {
        _userService = userService;
        _currentUserService = currentUserService;
        _passwordHasher = passwordHasher;
        _authTokenService = authTokenService;
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

        var user = await _userService.GetUserByUsernameAsync(username);
        if (user == null)
        {
            return (false, "Неверное имя пользователя или пароль");
        }

        if (!_passwordHasher.VerifyPassword(password, user.PasswordHash))
        {
            return (false, "Неверное имя пользователя или пароль");
        }

        // Получаем ID железа
        var hardwareId = await _hardwareInfoService.GetHardwareIdAsync();

        // Создаем токен авторизации
        var token = await _authTokenService.CreateTokenAsync(user.Id, hardwareId);

        // Сохраняем токен в настройках пользователя
        var settings = new UserSettings { AuthToken = token.Token };
        await _userSettingsService.SaveSettingsAsync(user.Id, settings);

        // Сохраняем ID пользователя в настройках приложения
        _appSettingsService.Settings.LoggedInUserId = user.Id;
        await _appSettingsService.SaveSettings();

        _currentUserService.SetCurrentUser(user);
        return (true, token.Token);
    }

    public async Task<(bool success, string? error)> RestoreSessionAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return (false, "Токен не может быть пустым");
        }

        var hardwareId = await _hardwareInfoService.GetHardwareIdAsync();
        var authToken = await _authTokenService.ValidateTokenAsync(token, hardwareId);

        if (authToken == null)
        {
            return (false, "Недействительный токен");
        }

        // Обновляем ID пользователя в настройках приложения
        _appSettingsService.Settings.LoggedInUserId = authToken.User.Id;
        await _appSettingsService.SaveSettings();

        _currentUserService.SetCurrentUser(authToken.User);
        return (true, null);
    }

    public async void Logout()
    {
        var currentUser = _currentUserService.CurrentUser;
        if (currentUser != null)
        {
            await _authTokenService.DeactivateAllUserTokensAsync(currentUser.Id);
        }

        // Очищаем ID пользователя в настройках приложения
        _appSettingsService.Settings.LoggedInUserId = 0;
        await _appSettingsService.SaveSettings();

        _currentUserService.SetCurrentUser(null);
    }

    public async Task<bool> TryRestoreSessionAsync()
    {
        // Загружаем настройки приложения для обеспечения актуальности данных
        await _appSettingsService.LoadSettings();
        
        // Проверяем наличие ID пользователя в настройках приложения
        int userId = _appSettingsService.Settings.LoggedInUserId;
        if (userId <= 0)
        {
            return false;
        }
        
        // Пробуем найти пользователя по ID
        var user = await _userService.GetUserByIdAsync(userId);
        if (user == null)
        {
            // Пользователь не найден в базе, сбрасываем настройки
            _appSettingsService.Settings.LoggedInUserId = 0;
            await _appSettingsService.SaveSettings();
            return false;
        }
        
        // Загружаем настройки пользователя, чтобы получить токен
        var userSettings = await _userSettingsService.LoadSettingsAsync(userId);
        if (userSettings?.AuthToken == null)
        {
            // Токен не найден, сбрасываем настройки приложения
            _appSettingsService.Settings.LoggedInUserId = 0;
            await _appSettingsService.SaveSettings();
            return false;
        }
        
        // Восстанавливаем сессию с использованием токена
        var (success, _) = await RestoreSessionAsync(userSettings.AuthToken);
        
        // Если восстановление не удалось, сбрасываем настройки приложения
        if (!success)
        {
            _appSettingsService.Settings.LoggedInUserId = 0;
            await _appSettingsService.SaveSettings();
        }
        
        return success;
    }
} 