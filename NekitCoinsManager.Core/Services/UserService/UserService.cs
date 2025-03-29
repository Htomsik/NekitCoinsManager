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
    private readonly List<IUserObserver> _observers = new();
    private List<User> _users = new();
    private User? _currentUser;

    public UserService(AppDbContext dbContext, IPasswordHasherService passwordHasherService)
    {
        _dbContext = dbContext;
        _passwordHasherService = passwordHasherService;
        LoadUsers();
    }

    private void LoadUsers()
    {
        _users = _dbContext.Users.ToList();
    }

    public IEnumerable<User> GetUsers()
    {
        return _users;
    }

    public User? GetUserByUsername(string username)
    {
        return _users.FirstOrDefault(u => 
            u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
    }

    public User? GetCurrentUser()
    {
        return _currentUser;
    }

    public void SetCurrentUser(User? user)
    {
        _currentUser = user;
        NotifyObservers();
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

        if (_users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
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

        // Обновляем кэш пользователей
        LoadUsers();
        NotifyObservers();
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
        
        // Обновляем кэш пользователей
        _users.Remove(user);
        NotifyObservers();
        return (true, null);
    }

    public void Subscribe(IUserObserver observer)
    {
        if (!_observers.Contains(observer))
        {
            _observers.Add(observer);
        }
    }

    private void NotifyObservers()
    {
        foreach (var observer in _observers)
        {
            observer.OnUsersChanged();
        }
    }

    // Метод для обновления баланса пользователя
    public async Task UpdateUserBalance(int userId, decimal newBalance)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null)
        {
            throw new Exception("Пользователь не найден");
        }

        user.Balance = newBalance;
        await _dbContext.SaveChangesAsync();
        
        // Обновляем кэш пользователей
        LoadUsers();
        NotifyObservers();
    }
} 