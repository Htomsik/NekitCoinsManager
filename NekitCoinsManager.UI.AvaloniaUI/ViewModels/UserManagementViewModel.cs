using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NekitCoinsManager.Core.Models;
using NekitCoinsManager.Core.Services;
using NekitCoinsManager.Services;

namespace NekitCoinsManager.ViewModels;

public partial class UserManagementViewModel : ViewModelBase
{
    private readonly IUserService _userService;
    private readonly INotificationService _notificationService;
    private readonly IUserBalanceService _userBalanceService;
    private readonly INavigationService _navigationService;
    
    [ObservableProperty]
    private ObservableCollection<User> _users = new();
    
    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public UserManagementViewModel(
        IUserService userService,
        INotificationService notificationService,
        IUserBalanceService userBalanceService,
        INavigationService navigationService)
    {
        _userService = userService;
        _notificationService = notificationService;
        _userBalanceService = userBalanceService;
        _navigationService = navigationService;
        LoadUsers();
    }

    private async void LoadUsers()
    {
        var users = await _userService.GetUsersAsync();
        Users.Clear();
        
        foreach (var user in users)
        {
            var balances = await _userBalanceService.GetUserBalancesAsync(user.Id);
            user.Balances.Clear();
            foreach (var balance in balances)
            {
                user.Balances.Add(balance);
            }
            Users.Add(user);
        }
    }

    [RelayCommand]
    private async Task DeleteUser(User? user)
    {
        if (user == null)
        {
            _notificationService.ShowError("Выберите пользователя для удаления");
            return;
        }

        var (success, error) = await _userService.DeleteUserAsync(user.Id);
        
        if (!success)
        {
            _notificationService.ShowError(error ?? "Произошла ошибка при удалении пользователя");
            return;
        }

        _notificationService.ShowSuccess("Пользователь успешно удален");
        LoadUsers(); // Обновляем список после успешного удаления
    }
    
    [RelayCommand]
    private async Task ViewUserTokens(User? user)
    {
        if (user == null)
        {
            _notificationService.ShowError("Выберите пользователя для просмотра токенов");
            return;
        }
        
        // Используем универсальный метод навигации с параметрами
        await _navigationService.NavigateToWithParamsAsync(ViewType.UserTokens, user, ViewType.UserManagement);
    }
} 