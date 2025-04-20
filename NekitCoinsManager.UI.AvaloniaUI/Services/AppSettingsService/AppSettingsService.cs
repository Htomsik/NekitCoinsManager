using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using NekitCoinsManager.Core.Models;
using NekitCoinsManager.Models;

namespace NekitCoinsManager.Services;

public class AppSettingsService : IAppSettingsService
{
    public AppSettings Settings { get; private set; } = new AppSettings();

    public AppSettingsService()
    {
        Directory.CreateDirectory(SettingsConstants.SettingsDirectory);
    }

    public async Task LoadSettings()
    {
        var filePath = GetSettingsFilePath();
        if (!File.Exists(filePath))
        {
            Settings = new AppSettings();
            return;
        }

        try
        {
            var json = await File.ReadAllTextAsync(filePath);
            var settings = JsonSerializer.Deserialize<AppSettings>(json, SettingsConstants.JsonOptions);
            if (settings != null)
            {
                Settings = settings;
            }
        }
        catch (Exception)
        {
            // В случае ошибки создаем новые настройки
            Settings = new AppSettings();
        }
    }

    public async Task SaveSettings()
    {
        Directory.CreateDirectory(SettingsConstants.SettingsDirectory);
        var filePath = GetSettingsFilePath();
        var json = JsonSerializer.Serialize(Settings, SettingsConstants.JsonOptions);
        await File.WriteAllTextAsync(filePath, json);
    }

    public async Task DeleteSettings()
    {
        var filePath = GetSettingsFilePath();
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        
        // Сбрасываем настройки на значения по умолчанию
        Settings = new AppSettings();
    }

    private string GetSettingsFilePath()
    {
        return Path.Combine(SettingsConstants.SettingsDirectory, SettingsConstants.AppSettingsFileName);
    }
} 