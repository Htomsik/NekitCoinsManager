using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NekitCoinsManager.Core.Models;
using NekitCoinsManager.Core.Repositories;

namespace NekitCoinsManager.Core.Services;

/// <summary>
/// Реализация сервиса для работы с пользователями
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ITransactionRepository _transactionRepository;

    public UserService(
        IUserRepository userRepository,
        ITransactionRepository transactionRepository)
    {
        _userRepository = userRepository;
        _transactionRepository = transactionRepository;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<User>> GetUsersAsync()
    {
        return await _userRepository.GetAllAsync();
    }

    /// <inheritdoc />
    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _userRepository.GetByUsernameAsync(username);
    }

    /// <inheritdoc />
    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _userRepository.GetByIdAsync(userId);
    }

    /// <inheritdoc />
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
} 