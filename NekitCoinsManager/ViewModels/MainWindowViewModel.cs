using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using NekitCoinsManager.Core.Models;
using NekitCoinsManager.Core.Services;

namespace NekitCoinsManager.ViewModels;

public partial class MainWindowViewModel : ViewModelBase, ICurrentUserObserver
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IServiceProvider _serviceProvider;

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
        NotificationViewModel notificationViewModel)
    {
        _currentUserService = currentUserService;
        _serviceProvider = serviceProvider;
        _userMiniCardViewModel = userMiniCardViewModel;
        _notificationViewModel = notificationViewModel;
        
        _currentUserService.Subscribe(this);
        
        // Устанавливаем начальную view
        Navigate(ViewType.Login);
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