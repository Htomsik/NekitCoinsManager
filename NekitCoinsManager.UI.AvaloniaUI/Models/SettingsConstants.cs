using System;
using System.IO;
using System.Text.Json;

namespace NekitCoinsManager.Models;

/// <summary>
/// Константы для работы с настройками приложения
/// </summary>
public static class SettingsConstants
{
    /// <summary>
    /// Базовая директория для хранения всех настроек
    /// </summary>
    public static readonly string SettingsDirectory = Path.Combine(Environment.CurrentDirectory, "Data");
    
    /// <summary>
    /// Имя файла настроек пользователя
    /// </summary>
    public const string UserSettingsFileName = "settings.json";
    
    /// <summary>
    /// Имя файла настроек приложения
    /// </summary>
    public const string AppSettingsFileName = "appSettings.json";
    
    /// <summary>
    /// Стандартные настройки JSON сериализации
    /// </summary>
    public static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };
} 