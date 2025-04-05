using System.Collections.Generic;
using NekitCoinsManager.Core.Models;
using NekitCoinsManager.Models;

namespace NekitCoinsManager.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly List<ICurrentUserObserver> _observers = new();
    private readonly IUserSettingsService _userSettingsService;
    private User? _currentUser;
    private UserSettings _settings = new();

    public CurrentUserService(IUserSettingsService userSettingsService)
    {
        _userSettingsService = userSettingsService;
    }

    public User? CurrentUser => _currentUser;
    public UserSettings Settings => _settings;

    public async void SetCurrentUser(User? user)
    {
        _currentUser = user;
        if (user == null)
        {
            ResetSettings();
        }
        else
        {
            var loadedSettings = await _userSettingsService.LoadSettingsAsync(user.Id);
            if (loadedSettings != null)
            {
                _settings = loadedSettings;
            }
        }
        NotifyObservers();
    }

    public void Subscribe(ICurrentUserObserver observer)
    {
        if (!_observers.Contains(observer))
        {
            _observers.Add(observer);
        }
    }

    public void UpdateSettings(UserSettings settings)
    {
        _settings = settings;
        NotifyObservers();
    }

    public void ResetSettings()
    {
        _settings = new UserSettings();
        NotifyObservers();
    }

    private void NotifyObservers()
    {
        foreach (var observer in _observers)
        {
            observer.OnCurrentUserChanged();
        }
    }
} 