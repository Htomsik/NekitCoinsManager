using System.Collections.Generic;
using System.Threading.Tasks;
using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Core.Services;

public class AuthService : IAuthService
{
    private readonly IUserService _userService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPasswordHasherService _passwordHasher;
    private readonly List<IAuthObserver> _observers = new();

    public bool IsAuthenticated => _currentUserService.CurrentUser != null;

    public AuthService(
        IUserService userService,
        ICurrentUserService currentUserService,
        IPasswordHasherService passwordHasher)
    {
        _userService = userService;
        _currentUserService = currentUserService;
        _passwordHasher = passwordHasher;
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

        var user = await _userService.GetUserByUsernameAsync(username);
        if (user == null)
        {
            return (false, "Неверное имя пользователя или пароль");
        }

        if (!_passwordHasher.VerifyPassword(password, user.PasswordHash))
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