using System;
using System.Threading.Tasks;
using NekitCoinsManager.Core.Models;
using NekitCoinsManager.Core.Repositories;

namespace NekitCoinsManager.Core.Services;

/// <summary>
/// Реализация сервиса для аутентификации, авторизации и регистрации пользователей
/// </summary>
public class UserAuthService : IUserAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasherService _passwordHasherService;
    private readonly IMoneyOperationsManager _moneyOperationsManager;
    private readonly IAuthTokenService _authTokenService;

    public UserAuthService(
        IUserRepository userRepository,
        IPasswordHasherService passwordHasherService,
        IMoneyOperationsManager moneyOperationsManager,
        IAuthTokenService authTokenService)
    {
        _userRepository = userRepository;
        _passwordHasherService = passwordHasherService;
        _moneyOperationsManager = moneyOperationsManager;
        _authTokenService = authTokenService;
    }

    /// <inheritdoc />
    public async Task<(bool success, string? error)> VerifyPasswordAsync(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return (false, "Имя пользователя не может быть пустым");
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            return (false, "Пароль не может быть пустым");
        }

        var user = await _userRepository.GetByUsernameAsync(username);
        if (user == null)
        {
            return (false, "Неверное имя пользователя или пароль");
        }

        if (!_passwordHasherService.VerifyPassword(password, user.PasswordHash))
        {
            return (false, "Неверное имя пользователя или пароль");
        }

        return (true, null);
    }

    /// <inheritdoc />
    public async Task<(bool success, string? error, User? user, UserAuthToken? token)> AuthenticateUserAsync(string username, string password, string hardwareId)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return (false, "Имя пользователя не может быть пустым", null, null);
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            return (false, "Пароль не может быть пустым", null, null);
        }

        if (string.IsNullOrWhiteSpace(hardwareId))
        {
            return (false, "Идентификатор устройства не может быть пустым", null, null);
        }

        var user = await _userRepository.GetByUsernameAsync(username);
        if (user == null)
        {
            return (false, "Неверное имя пользователя или пароль", null, null);
        }

        if (!_passwordHasherService.VerifyPassword(password, user.PasswordHash))
        {
            return (false, "Неверное имя пользователя или пароль", null, null);
        }
        
        try
        {
            // Выходим из всех предыдущих сессий пользователя
            await LogoutAsync(user.Id, hardwareId);
            
            // Создаем новый токен аутентификации
            var token = await _authTokenService.CreateTokenAsync(user.Id, hardwareId);
            return (true, null, user, token);
        }
        catch (Exception ex)
        {
            // В случае ошибки при создании токена, возвращаем ошибку, но пользователь все еще успешно авторизован
            return (true, $"Пользователь авторизован, но произошла ошибка при создании токена: {ex.Message}", user, null);
        }
    }

    /// <inheritdoc />
    public async Task<(bool success, string? error)> RegisterUserAsync(string username, string password, string confirmPassword)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return (false, "Имя пользователя не может быть пустым");
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            return (false, "Пароль не может быть пустым");
        }

        if (password != confirmPassword)
        {
            return (false, "Пароли не совпадают");
        }

        // Валидируем пароль
        var (isPasswordValid, passwordError) = await _userRepository.ValidatePasswordAsync(password);
        if (!isPasswordValid)
        {
            string userError = passwordError switch
            {
                ErrorCode.UserPasswordTooShort => "Пароль должен содержать не менее 6 символов",
                ErrorCode.UserPasswordTooLong => "Пароль слишком длинный",
                ErrorCode.UserPasswordNotComplex => "Пароль должен содержать заглавные, строчные буквы и цифры",
                _ => "Некорректный пароль"
            };
            return (false, userError);
        }

        var user = new User
        {
            Username = username,
            PasswordHash = _passwordHasherService.HashPassword(password),
            CreatedAt = DateTime.UtcNow
        };

        // Валидируем пользователя перед созданием
        var (isUserValid, validationError) = await _userRepository.ValidateCreateAsync(user);
        if (!isUserValid)
        {
            string error = validationError switch
            {
                ErrorCode.UserUsernameEmpty => "Имя пользователя не может быть пустым",
                ErrorCode.UserUsernameTooShort => "Имя пользователя должно содержать не менее 3 символов",
                ErrorCode.UserUsernameTooLong => "Имя пользователя слишком длинное",
                ErrorCode.UserUsernameInvalidCharacters => "Имя пользователя может содержать только буквы, цифры и символ подчеркивания",
                ErrorCode.UserUsernameAlreadyExists => "Пользователь с таким именем уже существует",
                _ => "Ошибка при создании пользователя"
            };
            return (false, error);
        }

        // Добавляем пользователя
        await _userRepository.AddAsync(user);

        // Начисляем приветственный бонус
        var welcomeBonusResult = await _moneyOperationsManager.GrantWelcomeBonusAsync(new WelcomeBonusOperation
        {
            NewUserId = user.Id,
            Comment = $"Приветственный бонус для пользователя {user.Username}"
        });

        if (!welcomeBonusResult.Success)
        {
            // Логируем ошибку, но не отменяем регистрацию пользователя
            // TODO: Добавить логирование ошибки
            return (true, "Пользователь зарегистрирован, но не удалось начислить приветственный бонус");
        }
        
        return (true, null);
    }
    
    /// <inheritdoc />
    public async Task<(bool success, string? error, User? user)> RestoreSessionAsync(string token, string hardwareId)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return (false, "Токен не может быть пустым", null);
        }
        
        if (string.IsNullOrWhiteSpace(hardwareId))
        {
            return (false, "Идентификатор устройства не может быть пустым", null);
        }

        // Проверяем валидность токена
        var authToken = await _authTokenService.ValidateTokenAsync(token, hardwareId);
        if (authToken == null)
        {
            return (false, "Недействительный токен аутентификации", null);
        }

        // Получаем пользователя по id
        var user = await _userRepository.GetByIdAsync(authToken.UserId);
        if (user == null)
        {
            return (false, "Пользователь не найден", null);
        }

        return (true, null, user);
    }
    
    /// <inheritdoc />
    public async Task<(bool success, string? error)> LogoutAsync(int userId, string hardwareId)
    {
        if (userId <= 0)
        {
            return (false, "Некорректный идентификатор пользователя");
        }
        
        if (string.IsNullOrWhiteSpace(hardwareId))
        {
            return (false, "Идентификатор устройства не может быть пустым");
        }
        
        try
        {
            // Деактивируем все токены пользователя
            await _authTokenService.DeactivateAllUserTokensAsync(userId);
            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, $"Ошибка при выходе из системы: {ex.Message}");
        }
    }
} 