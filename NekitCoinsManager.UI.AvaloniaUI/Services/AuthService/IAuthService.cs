using System.Threading.Tasks;

namespace NekitCoinsManager.Services;

public interface IAuthService
{
    Task<(bool success, string? error)> LoginAsync(string username, string password);
    Task<(bool success, string? error)> TryRestoreSessionAsync();
    Task<(bool success, string? error)> LogoutAsync();
} 