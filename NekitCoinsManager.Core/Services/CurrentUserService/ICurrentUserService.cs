using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Core.Services;

public interface ICurrentUserObserver
{
    void OnCurrentUserChanged();
}

public interface ICurrentUserService
{
    User? CurrentUser { get; }
    void SetCurrentUser(User? user);
    void Subscribe(ICurrentUserObserver observer);
} 