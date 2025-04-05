using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MapsterMapper;
using NekitCoinsManager.Models;
using NekitCoinsManager.Services;
using NekitCoinsManager.Shared.DTO;
using NekitCoinsManager.Shared.HttpClient;

namespace NekitCoinsManager.ViewModels;

public partial class UserManagementViewModel : ViewModelBase
{
    private readonly IUserServiceClient _userServiceClient;
    private readonly INotificationService _notificationService;
    private readonly IUserBalanceServiceClient _userBalanceServiceClient;
    private readonly INavigationService _navigationService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;
    
    [ObservableProperty]
    private ObservableCollection<UserDto> _users = new();
    
    [ObservableProperty]
    private Dictionary<int, List<UserBalanceDto>> _userBalances = new();
    
    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public UserManagementViewModel(
        IUserServiceClient userServiceClient,
        INotificationService notificationService,
        IUserBalanceServiceClient userBalanceServiceClient,
        INavigationService navigationService,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _userServiceClient = userServiceClient;
        _notificationService = notificationService;
        _userBalanceServiceClient = userBalanceServiceClient;
        _navigationService = navigationService;
        _currentUserService = currentUserService;
        _mapper = mapper;
        LoadUsers();
    }

    private async void LoadUsers()
    {
        var userDtos = await _userServiceClient.GetUsersAsync();
        Users.Clear();
        UserBalances.Clear();
        
        foreach (var userDto in userDtos)
        {
            // Загружаем балансы через клиент
            var balanceDtos = await _userBalanceServiceClient.GetUserBalancesAsync(userDto.Id);
            userDto.Balances = balanceDtos.ToList();
            
            UserBalances[userDto.Id] = balanceDtos.ToList();
            Users.Add(userDto);
        }
    }

    [RelayCommand]
    private async Task DeleteUser(UserDto? user)
    {
        if (user == null)
        {
            _notificationService.ShowError("Выберите пользователя для удаления");
            return;
        }

        var result = await _userServiceClient.DeleteUserAsync(user.Id);
        
        if (!result.Success)
        {
            _notificationService.ShowError(result.Error ?? "Произошла ошибка при удалении пользователя");
            return;
        }

        _notificationService.ShowSuccess("Пользователь успешно удален");
        LoadUsers(); // Обновляем список после успешного удаления
    }
    
    [RelayCommand]
    private async Task ViewUserTokens(UserDto? user)
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