using System;
using NekitCoinsManager.Models;

namespace NekitCoinsManager.Services;

public interface INotificationService
{
    event Action<Notification>? NotificationReceived;
    
    void ShowError(string message);
    void ShowSuccess(string message);
    void ShowInfo(string message);
    void ShowWarning(string message);
} 