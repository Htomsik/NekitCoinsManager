using System;
using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Core.Services;

public interface INotificationService
{
    event Action<Notification>? NotificationReceived;
    
    void ShowError(string message);
    void ShowSuccess(string message);
    void ShowInfo(string message);
    void ShowWarning(string message);
} 