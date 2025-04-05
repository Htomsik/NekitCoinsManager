using NekitCoinsManager.Core.Models;
using NekitCoinsManager.Models;

namespace NekitCoinsManager.Services;

public interface ICurrentUserObserver
{
    void OnCurrentUserChanged();
}

public interface ICurrentUserService
{
    User? CurrentUser { get; }
    UserSettings Settings { get; }
    void SetCurrentUser(User? user);
    void Subscribe(ICurrentUserObserver observer);
    void UpdateSettings(UserSettings settings);
    void ResetSettings();
} 