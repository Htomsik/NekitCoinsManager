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
    /// Базовая директория для хранения настроек пользователей
    /// </summary>
    public static readonly string UserSettingsDirectory = Path.Combine(Environment.CurrentDirectory, "Data");
    
    /// <summary>
    /// Имя файла настроек пользователя
    /// </summary>
    public const string UserSettingsFileName = "userSettings.json";
    
    /// <summary>
    /// Базовая директория для хранения глобальных настроек
    /// </summary>
    public static readonly string AppSettingsDirectory = Environment.CurrentDirectory;
    
    /// <summary>
    /// Имя файла настроек приложения
    /// </summary>
    public const string AppSettingsFileName = "appSettings.json";
    
    /// <summary>
    ///  Имя глобального клиента ДЛЯ СРАНОЙ ФАБРИКИ
    /// </summary>
    public const string HttpClientName = "HttpClient";
    
    /// <summary>
    /// Стандартные настройки JSON сериализации
    /// </summary>
    public static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };
} 