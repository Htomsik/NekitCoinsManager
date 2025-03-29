using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NekitCoinsManager.Data;
using NekitCoinsManager.Models;

namespace NekitCoinsManager.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _dbContext;
    private readonly List<IAuthObserver> _observers = new();
    private User? _currentUser;

    public User? CurrentUser => _currentUser;
    public bool IsAuthenticated => _currentUser != null;

    public AuthService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
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

        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);

        if (user == null)
        {
            return (false, "Неверное имя пользователя или пароль");
        }

        _currentUser = user;
        NotifyObservers();
        return (true, null);
    }

    public void Logout()
    {
        _currentUser = null;
        NotifyObservers();
    }

    public void Subscribe(IAuthObserver observer)
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
            observer.OnAuthStateChanged(_currentUser);
        }
    }
} 