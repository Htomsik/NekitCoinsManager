using System;
using System.IO;

namespace NekitCoinsManager.Core.Models;

/// <summary>
/// Константы для работы с настройками приложения
/// </summary>
internal static class SettingsConstants
{
    /// <summary>
    /// Базовая директория для хранения всех настроек
    /// </summary>
    public static readonly string SettingsDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), 
        "NekitCoinsManager");
}