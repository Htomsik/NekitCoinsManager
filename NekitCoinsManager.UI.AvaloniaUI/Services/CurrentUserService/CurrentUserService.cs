using System.Collections.Generic;
using NekitCoinsManager.Models;
using NekitCoinsManager.Shared.DTO;

namespace NekitCoinsManager.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly List<ICurrentUserObserver> _observers = new();
    private readonly IUserSettingsService _userSettingsService;
    private UserDto? _currentUser;
    private UserSettings _settings = new();

    public CurrentUserService(IUserSettingsService userSettingsService)
    {
        _userSettingsService = userSettingsService;
    }

    public UserDto? CurrentUser => _currentUser;
    public UserSettings Settings => _settings;

    public async void SetCurrentUser(UserDto? user)
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