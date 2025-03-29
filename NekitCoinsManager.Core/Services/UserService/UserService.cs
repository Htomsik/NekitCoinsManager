using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NekitCoinsManager.Core.Data;
using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Core.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _dbContext;
    private readonly IPasswordHasherService _passwordHasherService;

    public UserService(AppDbContext dbContext, IPasswordHasherService passwordHasherService)
    {
        _dbContext = dbContext;
        _passwordHasherService = passwordHasherService;
    }

    public async Task<IEnumerable<User>> GetUsersAsync()
    {
        return await _dbContext.Users.ToListAsync();
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Username.Equals(username));
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

        var existingUser = await GetUserByUsernameAsync(username);
        if (existingUser != null)
        {
            return (false, "Пользователь с таким именем уже существует");
        }

        var user = new User
        {
            Username = username,
            PasswordHash = _passwordHasherService.HashPassword(password),
            Balance = 100, // Начальный баланс для нового пользователя
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        return (true, null);
    }

    public async Task<(bool success, string? error)> DeleteUserAsync(int userId)
    {
        var user = await _dbContext.Users
            .Include(u => u.SentTransactions)
            .Include(u => u.ReceivedTransactions)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return (false, "Пользователь не найден");
        }

        if (user.SentTransactions.Any() || user.ReceivedTransactions.Any())
        {
            return (false, "Невозможно удалить пользователя с историей транзакций");
        }

        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();
        
        return (true, null);
    }

    public async Task UpdateUserBalance(int userId, decimal newBalance)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null)
        {
            throw new Exception("Пользователь не найден");
        }

        user.Balance = newBalance;
        await _dbContext.SaveChangesAsync();
    }
} 