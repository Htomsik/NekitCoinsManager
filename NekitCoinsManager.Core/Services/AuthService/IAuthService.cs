using System.Threading.Tasks;
using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Core.Services;

public interface IAuthObserver
{
    void OnAuthStateChanged();
}

public interface IAuthService
{
    bool IsAuthenticated { get; }
    Task<(bool success, string? error)> LoginAsync(string username, string password);
    void Logout();
    void Subscribe(IAuthObserver observer);
} 