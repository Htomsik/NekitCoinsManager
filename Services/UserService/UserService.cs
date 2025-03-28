using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NekitCoinsManager.Data;
using NekitCoinsManager.Models;

namespace NekitCoinsManager.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _dbContext;
    private readonly List<IUserObserver> _observers = new();
    private List<User> _users = new();

    public UserService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
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

    public async Task AddUser(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new Exception("Имя пользователя не может быть пустым");
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            throw new Exception("Пароль не может быть пустым");
        }

        if (_users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
        {
            throw new Exception("Пользователь с таким именем уже существует");
        }

        var user = new User
        {
            Username = username,
            Password = password, // В реальном приложении пароль должен быть захеширован
            Balance = 100, // Начальный баланс для нового пользователя
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        // Обновляем кэш пользователей
        LoadUsers();
        NotifyObservers();
    }

    public async Task DeleteUser(int userId)
    {
        var user = await _dbContext.Users
            .Include(u => u.SentTransactions)
            .Include(u => u.ReceivedTransactions)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            throw new Exception("Пользователь не найден");
        }

        if (user.SentTransactions.Any() || user.ReceivedTransactions.Any())
        {
            throw new Exception("Невозможно удалить пользователя с историей транзакций");
        }

        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();
        
        // Обновляем кэш пользователей
        _users.Remove(user);
        NotifyObservers();
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