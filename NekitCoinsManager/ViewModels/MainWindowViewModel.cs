using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using NekitCoinsManager.Core.Services;

namespace NekitCoinsManager.ViewModels;

public partial class MainWindowViewModel : ViewModelBase, ICurrentUserObserver
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IAuthService _authService;

    [ObservableProperty]
    private object _currentView;

    [ObservableProperty]
    private UserMiniCardViewModel _userMiniCardViewModel;

    [ObservableProperty]
    private NotificationViewModel _notificationViewModel;

    public bool IsAuthenticated => _currentUserService.CurrentUser != null;

    public MainWindowViewModel(
        ICurrentUserService currentUserService,
        IServiceProvider serviceProvider,
        UserMiniCardViewModel userMiniCardViewModel,
        NotificationViewModel notificationViewModel,
        IAuthService authService)
    {
        _currentUserService = currentUserService;
        _serviceProvider = serviceProvider;
        _userMiniCardViewModel = userMiniCardViewModel;
        _notificationViewModel = notificationViewModel;
        _authService = authService;
        
        _currentUserService.Subscribe(this);
        
        // Пытаемся восстановить сессию
        TryRestoreSessionAsync();
    }
    
    private async void TryRestoreSessionAsync()
    {
        var success = await _authService.TryRestoreSessionAsync();
        if (!success)
        {
            // Если не удалось восстановить сессию, показываем форму входа
            Navigate(ViewType.Login);
        }
    }
    
    [RelayCommand]
    private void Navigate(ViewType viewType)
    {
        CurrentView = viewType switch
        {
            ViewType.Login => _serviceProvider.GetRequiredService<UserLoginViewModel>(),
            ViewType.Registration => _serviceProvider.GetRequiredService<UserRegistrationViewModel>(),
            ViewType.UserManagement => _serviceProvider.GetRequiredService<UserManagementViewModel>(),
            ViewType.Transaction => _serviceProvider.GetRequiredService<TransactionViewModel>(),
            ViewType.TransactionHistory => _serviceProvider.GetRequiredService<TransactionHistoryViewModel>(),
            ViewType.UserCard => _serviceProvider.GetRequiredService<UserCardViewModel>(),
            ViewType.CurrencyManagement => _serviceProvider.GetRequiredService<CurrencyManagementViewModel>(),
            _ => throw new ArgumentException($"Unknown view type: {viewType}")
        };
    }
    
    public void OnCurrentUserChanged()
    {
        // Обновляем view в зависимости от состояния авторизации
        Navigate(_currentUserService.CurrentUser != null ? ViewType.Transaction : ViewType.Login);
        
        // Уведомляем UI об изменении состояния авторизации
        OnPropertyChanged(nameof(IsAuthenticated));
    }
}