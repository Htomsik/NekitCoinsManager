using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NekitCoinsManager.Core.Data;
using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Core.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly List<IAuthObserver> _observers = new();

    public bool IsAuthenticated => _currentUserService.CurrentUser != null;

    public AuthService(AppDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
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
            .FirstOrDefaultAsync(u => u.Username == username);

        if (user == null)
        {
            return (false, "Неверное имя пользователя или пароль");
        }

        _currentUserService.SetCurrentUser(user);
        NotifyObservers();
        return (true, null);
    }

    public void Logout()
    {
        _currentUserService.SetCurrentUser(null);
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
            observer.OnAuthStateChanged();
        }
    }
} 