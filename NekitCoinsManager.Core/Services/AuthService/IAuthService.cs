using System.Threading.Tasks;
using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Core.Services;

public interface IAuthService
{
    Task<(bool success, string? error)> LoginAsync(string username, string password);
    void Logout();
} 