using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using NekitCoinsManager.Models;
using NekitCoinsManager.Services;

namespace NekitCoinsManager.ViewModels;

public partial class NotificationViewModel : ViewModelBase
{
    private readonly INotificationService _notificationService;
    
    [ObservableProperty]
    private Notification? _currentNotification;
    
    [ObservableProperty]
    private bool _isVisible;
    

    public NotificationViewModel(INotificationService notificationService)
    {
        _notificationService = notificationService;
        _notificationService.NotificationReceived += OnNotificationReceived;
        
        // Устанавливаем начальные значения
        IsVisible = false;
    }

    private async void OnNotificationReceived(Notification notification)
    {
        CurrentNotification = notification;

        IsVisible = true;
        // Автоматически скрываем уведомление через 5 секунд
        await Task.Delay(5000);
        if (CurrentNotification == notification) // Проверяем, что уведомление не изменилось
        {
            IsVisible = false;
        }
    }
    
    public void Close()
    {
        IsVisible = false;
    }
} 