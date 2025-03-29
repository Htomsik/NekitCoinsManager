using System.Threading.Tasks;
using NekitCoinsManager.Models;

namespace NekitCoinsManager.Services;

public interface IAuthObserver
{
    void OnAuthStateChanged(User? user);
}

public interface IAuthService
{
    User? CurrentUser { get; }
    bool IsAuthenticated { get; }
    Task<(bool success, string? error)> LoginAsync(string username, string password);
    void Logout();
    void Subscribe(IAuthObserver observer);
} 