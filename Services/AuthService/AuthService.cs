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

    public async Task<bool> LoginAsync(string username, string password)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);

        if (user == null)
        {
            return false;
        }

        _currentUser = user;
        NotifyObservers();
        return true;
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