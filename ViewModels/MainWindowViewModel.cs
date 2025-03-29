using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using NekitCoinsManager.Core.Models;
using NekitCoinsManager.Core.Services;

namespace NekitCoinsManager.ViewModels;

public partial class MainWindowViewModel : ViewModelBase, IAuthObserver
{
    private readonly IAuthService _authService;
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    private object _currentView;

    [ObservableProperty]
    private bool _isAuthenticated;

    [ObservableProperty]
    private UserMiniCardViewModel _userMiniCardViewModel;

    public MainWindowViewModel(
        IAuthService authService,
        IServiceProvider serviceProvider,
        UserMiniCardViewModel userMiniCardViewModel)
    {
        _authService = authService;
        _serviceProvider = serviceProvider;
        _userMiniCardViewModel = userMiniCardViewModel;
        
        _authService.Subscribe(this);
        
        // Устанавливаем начальную view
        NavigateTo(ViewType.Login);
    }

    private void NavigateTo(ViewType viewType)
    {
        // Получаем инстанс ViewModel через DI в зависимости от типа
        CurrentView = viewType switch
        {
            ViewType.Login => _serviceProvider.GetRequiredService<UserLoginViewModel>(),
            ViewType.Registration => _serviceProvider.GetRequiredService<UserRegistrationViewModel>(),
            ViewType.UserManagement => _serviceProvider.GetRequiredService<UserManagementViewModel>(),
            ViewType.Transaction => _serviceProvider.GetRequiredService<TransactionViewModel>(),
            ViewType.TransactionHistory => _serviceProvider.GetRequiredService<TransactionHistoryViewModel>(),
            ViewType.UserCard => _serviceProvider.GetRequiredService<UserCardViewModel>(),
            _ => throw new ArgumentException($"Неизвестный тип представления: {viewType}")
        };
    }

    [RelayCommand]
    private void Navigate(ViewType viewType) => NavigateTo(viewType);
    
    public void OnAuthStateChanged()
    {
        IsAuthenticated = _authService.IsAuthenticated;
        
        // Если пользователь вышел из системы, перенаправляем на страницу входа
        if (!IsAuthenticated)
        {
            NavigateTo(ViewType.Login);
        }
        else
        {
            NavigateTo(ViewType.Transaction);
        }
    }
}