using System;
using NekitCoinsManager.Models;

namespace NekitCoinsManager.Services;

public class NotificationService : INotificationService
{
    public event Action<Notification>? NotificationReceived;

    private void Show(string message, NotificationType type)
    {
        var notification = new Notification
        {
            Message = message,
            Type = type
        };
        
        NotificationReceived?.Invoke(notification);
    }

    public void ShowError(string message) => Show(message, NotificationType.Error);
    public void ShowSuccess(string message) => Show(message, NotificationType.Success);
    public void ShowInfo(string message) => Show(message, NotificationType.Info);
    public void ShowWarning(string message) => Show(message, NotificationType.Warning);
} 