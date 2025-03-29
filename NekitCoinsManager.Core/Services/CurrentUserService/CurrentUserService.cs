using System.Collections.Generic;
using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Core.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly List<ICurrentUserObserver> _observers = new();
    private User? _currentUser;

    public User? CurrentUser => _currentUser;

    public void SetCurrentUser(User? user)
    {
        _currentUser = user;
        NotifyObservers();
    }

    public void Subscribe(ICurrentUserObserver observer)
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
            observer.OnCurrentUserChanged();
        }
    }
} 