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

    public UserService(
        IUserRepository userRepository,
        ITransactionRepository transactionRepository,
        IPasswordHasherService passwordHasherService)
    {
        _userRepository = userRepository;
        _transactionRepository = transactionRepository;
        _passwordHasherService = passwordHasherService;
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
        
        return (true, null);
    }

    public async Task<(bool success, string? error)> DeleteUserAsync(int userId)
    {
        // Валидируем удаление пользователя
        var (canDelete, deleteError) = await _userRepository.ValidateDeleteAsync(userId);
        if (!canDelete)
        {
            // Преобразуем технические коды ошибок в понятные пользователю сообщения
            string userError = deleteError switch
            {
                ErrorCode.CommonEntityNotFound => "Пользователь не найден",
                ErrorCode.UserNotFound => "Пользователь не найден",
                ErrorCode.UserCannotDeleteBankAccount => "Нельзя удалить банковский аккаунт",
                ErrorCode.UserHasBalances => "Пользователь имеет балансы, сначала необходимо их удалить",
                _ => "Ошибка при удалении пользователя"
            };
            return (false, userError);
        }

        var user = await _userRepository.GetByIdAsync(userId);
        await _userRepository.DeleteAsync(user!);
        
        return (true, null);
    }
} 