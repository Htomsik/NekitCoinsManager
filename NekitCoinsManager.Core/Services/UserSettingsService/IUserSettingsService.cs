using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Core.Services;

public interface IUserSettingsService
{
    Task SaveSettingsAsync(int userId, UserSettings settings);
    Task<UserSettings?> LoadSettingsAsync(int userId);
    Task DeleteSettingsAsync(int userId);
} 