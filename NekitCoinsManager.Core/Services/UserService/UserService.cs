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
    private readonly ITransactionService _transactionService;

    public UserService(
        IUserRepository userRepository,
        ITransactionRepository transactionRepository,
        IPasswordHasherService passwordHasherService,
        ITransactionService transactionService)
    {
        _userRepository = userRepository;
        _transactionRepository = transactionRepository;
        _passwordHasherService = passwordHasherService;
        _transactionService = transactionService;
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

        // Проверяем уникальность имени пользователя
        var isUsernameUnique = await _userRepository.IsUsernameUniqueAsync(username);
        if (!isUsernameUnique)
        {
            return (false, "Пользователь с таким именем уже существует");
        }

        var user = new User
        {
            Username = username,
            PasswordHash = _passwordHasherService.HashPassword(password),
            CreatedAt = DateTime.UtcNow
        };

        // Добавляем пользователя
        await _userRepository.AddAsync(user);

        // Выдаем приветственный бонус
        var (bonusSuccess, bonusError) = await _transactionService.GrantWelcomeBonusAsync(user.Id);
        if (!bonusSuccess)
        {
            // Если не удалось выдать бонус, удаляем пользователя
            await _userRepository.DeleteAsync(user);
            return (false, bonusError);
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
        
        if (user.IsBankAccount)
        {
            return (false, "Невозможно удалить системный банковский аккаунт");
        }

        // Проверяем наличие транзакций у пользователя
        var sentTransactions = await _transactionRepository.GetUserSentTransactionsAsync(userId);
        var receivedTransactions = await _transactionRepository.GetUserReceivedTransactionsAsync(userId);
        
        if (sentTransactions.Any() || receivedTransactions.Any())
        {
            return (false, "Невозможно удалить пользователя с историей транзакций");
        }

        await _userRepository.DeleteAsync(user);
        return (true, null);
    }
} 