using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NekitCoinsManager.Core.Models;
using NekitCoinsManager.Core.Services;

namespace NekitCoinsManager.ViewModels;

public partial class UserCardViewModel : ViewModelBase
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthService _authService;
    private readonly INotificationService _notificationService;
    
    [ObservableProperty]
    private User? _currentUser;
    
    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public UserCardViewModel(
        ICurrentUserService currentUserService, 
        IAuthService authService,
        INotificationService notificationService)
    {
        _currentUserService = currentUserService;
        _authService = authService;
        _notificationService = notificationService;
        LoadCurrentUser();
    }

    private void LoadCurrentUser()
    {
        CurrentUser = _currentUserService.CurrentUser;
    }

    [RelayCommand]
    private void Logout()
    {
        _authService.Logout();
        _notificationService.ShowInfo("Выход выполнен успешно");
    }
} 