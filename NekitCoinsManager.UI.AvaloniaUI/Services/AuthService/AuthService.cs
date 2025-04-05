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

        // Получаем пользователя по имени
        var user = await _userServiceClient.GetUserByUsernameAsync(username);
        if (user == null)
        {
            return (false, "Неверное имя пользователя или пароль");
        }
        
        // Проверяем пароль пользователя с помощью нового метода
        var verifyResult = await _userServiceClient.VerifyPasswordAsync(username, password);
        if (!verifyResult.Success)
        {
            return (false, "Неверное имя пользователя или пароль");
        }

        // Получаем ID железа
        var hardwareId = await _hardwareInfoService.GetHardwareIdAsync();

        // Создаем токен авторизации
        var tokenDto = await _authTokenServiceClient.CreateTokenAsync(user.Id, hardwareId);

        // Сохраняем токен в настройках пользователя
        var settings = new UserSettings { AuthToken = tokenDto.Token };
        await _userSettingsService.SaveSettingsAsync(user.Id, settings);

        // Сохраняем ID пользователя в настройках приложения
        _appSettingsService.Settings.LoggedInUserId = user.Id;
        await _appSettingsService.SaveSettings();

        _currentUserService.SetCurrentUser(user);
        return (true, tokenDto.Token);
    }

    public async Task<(bool success, string? error)> RestoreSessionAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return (false, "Токен не может быть пустым");
        }

        var hardwareId = await _hardwareInfoService.GetHardwareIdAsync();
        var authTokenDto = await _authTokenServiceClient.ValidateTokenAsync(token, hardwareId);

        if (authTokenDto == null)
        {
            return (false, "Недействительный токен");
        }

        // Получаем пользователя по id, потому что UserAuthTokenDto не содержит информацию о пользователе
        var user = await _userServiceClient.GetUserByIdAsync(authTokenDto.UserId);
        if (user == null)
        {
            return (false, "Пользователь не найден");
        }

        // Обновляем ID пользователя в настройках приложения
        _appSettingsService.Settings.LoggedInUserId = user.Id;
        await _appSettingsService.SaveSettings();

        _currentUserService.SetCurrentUser(user);
        return (true, null);
    }

    public async void Logout()
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
        var user = await _userServiceClient.GetUserByIdAsync(userId);
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