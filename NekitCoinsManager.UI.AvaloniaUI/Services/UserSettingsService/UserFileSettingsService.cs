using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using NekitCoinsManager.Core.Models;
using NekitCoinsManager.Models;

namespace NekitCoinsManager.Services;

public class UserFileSettingsService : IUserSettingsService
{
    public UserFileSettingsService()
    {
        Directory.CreateDirectory(SettingsConstants.SettingsDirectory);
    }

    public async Task SaveSettingsAsync(int userId, UserSettings settings)
    {
        var userDirectory = GetUserDirectory(userId);
        Directory.CreateDirectory(userDirectory);
        
        var filePath = Path.Combine(userDirectory, SettingsConstants.UserSettingsFileName);
        var json = JsonSerializer.Serialize(settings, SettingsConstants.JsonOptions);
        await File.WriteAllTextAsync(filePath, json);
    }

    public async Task<UserSettings?> LoadSettingsAsync(int userId)
    {
        var filePath = GetSettingsFilePath(userId);
        if (!File.Exists(filePath))
        {
            return new UserSettings();
        }

        var json = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<UserSettings>(json, SettingsConstants.JsonOptions);
    }

    public async Task DeleteSettingsAsync(int userId)
    {
        var userDirectory = GetUserDirectory(userId);
        if (Directory.Exists(userDirectory))
        {
            Directory.Delete(userDirectory, true);
        }
    }

    private string GetUserDirectory(int userId)
    {
        return Path.Combine(SettingsConstants.SettingsDirectory, userId.ToString());
    }

    private string GetSettingsFilePath(int userId)
    {
        return Path.Combine(GetUserDirectory(userId), SettingsConstants.UserSettingsFileName);
    }
} 