using System.Collections.ObjectModel;
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
    private readonly IUserBalanceService _userBalanceService;
    
    [ObservableProperty]
    private User? _currentUser;
    
    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private ObservableCollection<UserBalance> _balances = new();

    public UserCardViewModel(
        ICurrentUserService currentUserService, 
        IAuthService authService,
        INotificationService notificationService,
        IUserBalanceService userBalanceService)
    {
        _currentUserService = currentUserService;
        _authService = authService;
        _notificationService = notificationService;
        _userBalanceService = userBalanceService;
        LoadCurrentUser();
        LoadBalancesAsync();
    }

    private void LoadCurrentUser()
    {
        CurrentUser = _currentUserService.CurrentUser;
    }

    private async void LoadBalancesAsync()
    {
        if (CurrentUser == null) return;

        var balances = await _userBalanceService.GetUserBalancesAsync(CurrentUser.Id);
        Balances.Clear();
        foreach (var balance in balances)
        {
            Balances.Add(balance);
        }
    }

    [RelayCommand]
    private void Logout()
    {
        _authService.Logout();
        _notificationService.ShowInfo("Выход выполнен успешно");
    }
} 