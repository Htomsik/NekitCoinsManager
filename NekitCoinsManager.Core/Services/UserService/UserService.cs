using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NekitCoinsManager.Core.Data;
using NekitCoinsManager.Core.Models;
using NekitCoinsManager.Core.Repositories;

namespace NekitCoinsManager.Core.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IPasswordHasherService _passwordHasherService;
    private readonly IMoneyOperationsManager _moneyOperationsManager;

    public UserService(
        IUserRepository userRepository,
        ITransactionRepository transactionRepository,
        IPasswordHasherService passwordHasherService,
        IMoneyOperationsManager moneyOperationsManager)
    {
        _userRepository = userRepository;
        _transactionRepository = transactionRepository;
        _passwordHasherService = passwordHasherService;
        _moneyOperationsManager = moneyOperationsManager;
    }

    public async Task<IEnumerable<User>> GetUsersAsync()
    {
        return await _userRepository.GetAllAsync();
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _userRepository.GetByUsernameAsync(username);
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _userRepository.GetByIdAsync(userId);
    }

    public async Task<(bool success, string? error)> AddUserAsync(string username, string password, string confirmPassword)
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

    public async Task<(bool success, string? error)> DeleteUserAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return (false, "Пользователь не найден");
        }

        var transactions = await _transactionRepository.FindAsync(t => t.FromUserId == userId || t.ToUserId == userId);
        if (transactions.Any())
        {
            return (false, "Невозможно удалить пользователя с историей транзакций");
        }

        await _userRepository.DeleteAsync(user);
        return (true, null);
    }

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

    public async Task<(bool success, string? error, User? user)> AuthenticateUserAsync(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return (false, "Имя пользователя не может быть пустым", null);
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            return (false, "Пароль не может быть пустым", null);
        }

        var user = await _userRepository.GetByUsernameAsync(username);
        if (user == null)
        {
            return (false, "Неверное имя пользователя или пароль", null);
        }

        if (!_passwordHasherService.VerifyPassword(password, user.PasswordHash))
        {
            return (false, "Неверное имя пользователя или пароль", null);
        }

        return (true, null, user);
    }
} 