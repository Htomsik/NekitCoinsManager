using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NekitCoinsManager.Core.Models;
using NekitCoinsManager.Services;
using NekitCoinsManager.Shared.DTO;
using NekitCoinsManager.Shared.HttpClient;

namespace NekitCoinsManager.ViewModels;

public partial class UserTokensViewModel : ViewModelBase
{
    private readonly IAuthTokenServiceClient _authTokenServiceClient;
    private readonly INotificationService _notificationService;
    private readonly INavigationService _navigationService;
    
    [ObservableProperty]
    private UserDto? _user;
    
    [ObservableProperty]
    private ObservableCollection<UserAuthTokenDto> _tokens = new();
    
    public UserTokensViewModel(
        IAuthTokenServiceClient authTokenServiceClient,
        INotificationService notificationService,
        INavigationService navigationService)
    {
        _authTokenServiceClient = authTokenServiceClient;
        _notificationService = notificationService;
        _navigationService = navigationService;
    }
    
    public async Task LoadUserTokens(UserDto user)
    {
        User = user;
        await RefreshTokensAsync();
    }
    
    private async Task RefreshTokensAsync()
    {
        if (User == null) return;
        
        Tokens.Clear();
        var tokens = await _authTokenServiceClient.GetUserTokensAsync(User.Id);
        
        foreach (var token in tokens)
        {
            Tokens.Add(token);
        }
    }
    
    [RelayCommand]
    private async Task DeactivateToken(UserAuthTokenDto? token)
    {
        if (token == null)
        {
            _notificationService.ShowError("Выберите токен для деактивации");
            return;
        }
        
        await _authTokenServiceClient.DeactivateTokenAsync(token.Id);
        _notificationService.ShowSuccess("Токен деактивирован");
        
        // Обновляем список токенов
        await RefreshTokensAsync();
    }
    
    [RelayCommand]
    private void GoBack()
    {
        // Возвращаемся к предыдущему экрану (карточка пользователя или список пользователей)
        _navigationService.NavigateBack();
    }
} 