using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NekitCoinsManager.Models;
using NekitCoinsManager.Services;
using NekitCoinsManager.Shared.DTO;
using NekitCoinsManager.Shared.HttpClient;

namespace NekitCoinsManager.ViewModels;

public partial class UserCardViewModel : ViewModelBase
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthService _authService;
    private readonly INotificationService _notificationService;
    private readonly IUserBalanceServiceClient _userBalanceServiceClient;
    private readonly INavigationService _navigationService;
    
    [ObservableProperty]
    private UserDto? _currentUser;
    
    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private ObservableCollection<UserBalanceDto> _balances = new();

    public UserCardViewModel(
        ICurrentUserService currentUserService, 
        IAuthService authService,
        INotificationService notificationService,
        IUserBalanceServiceClient userBalanceServiceClient,
        INavigationService navigationService)
    {
        _currentUserService = currentUserService;
        _authService = authService;
        _notificationService = notificationService;
        _userBalanceServiceClient = userBalanceServiceClient;
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

        var balanceDtos = await _userBalanceServiceClient.GetUserBalancesAsync(CurrentUser.Id);
        Balances.Clear();
        
        foreach (var balanceDto in balanceDtos)
        {
            Balances.Add(balanceDto);
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