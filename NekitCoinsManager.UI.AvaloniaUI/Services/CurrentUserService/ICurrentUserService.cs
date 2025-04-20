using NekitCoinsManager.Models;
using NekitCoinsManager.Shared.DTO;

namespace NekitCoinsManager.Services;

public interface ICurrentUserObserver
{
    void OnCurrentUserChanged();
}

public interface ICurrentUserService
{
    UserDto? CurrentUser { get; }
    UserSettings Settings { get; }
    void SetCurrentUser(UserDto? user);
    void Subscribe(ICurrentUserObserver observer);
    void UpdateSettings(UserSettings settings);
    void ResetSettings();
} 