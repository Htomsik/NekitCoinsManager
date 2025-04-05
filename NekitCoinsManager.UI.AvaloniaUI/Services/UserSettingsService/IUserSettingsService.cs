using System.Threading.Tasks;
using NekitCoinsManager.Models;

namespace NekitCoinsManager.Services;

public interface IUserSettingsService
{
    Task SaveSettingsAsync(int userId, UserSettings settings);
    Task<UserSettings?> LoadSettingsAsync(int userId);
    Task DeleteSettingsAsync(int userId);
} 