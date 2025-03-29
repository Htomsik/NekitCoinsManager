using System;

namespace NekitCoinsManager.Core.Models;

public class Notification
{
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

public enum NotificationType
{
    Error,
    Success,
    Info,
    Warning
} 