using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NekitCoinsManager.Core.Models;
using NekitCoinsManager.Core.Services;
using NekitCoinsManager.Models;
using NekitCoinsManager.Services;

namespace NekitCoinsManager.ViewModels;

public partial class UserCardViewModel : ViewModelBase
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthService _authService;
    private readonly INotificationService _notificationService;
    private readonly IUserBalanceService _userBalanceService;
    private readonly INavigationService _navigationService;
    
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
        IUserBalanceService userBalanceService,
        INavigationService navigationService)
    {
        _currentUserService = currentUserService;
        _authService = authService;
        _notificationService = notificationService;
        _userBalanceService = userBalanceService;
        _navigationService = navigationService;
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
    
    [RelayCommand]
    private async Task ViewUserTokens()
    {
        if (CurrentUser == null)
        {
            _notificationService.ShowError("Пользователь не найден");
            return;
        }
        
        // Используем универсальный метод навигации с параметрами
        await _navigationService.NavigateToWithParamsAsync(ViewType.UserTokens, CurrentUser, ViewType.UserCard);
    }
} 