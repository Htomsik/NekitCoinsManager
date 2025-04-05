using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NekitCoinsManager.Core.Models;
using NekitCoinsManager.Core.Services;
using NekitCoinsManager.Services;

namespace NekitCoinsManager.ViewModels;

public partial class UserTokensViewModel : ViewModelBase
{
    private readonly IAuthTokenService _authTokenService;
    private readonly INotificationService _notificationService;
    private readonly INavigationService _navigationService;
    
    [ObservableProperty]
    private User? _user;
    
    [ObservableProperty]
    private ObservableCollection<UserAuthToken> _tokens = new();
    
    public UserTokensViewModel(
        IAuthTokenService authTokenService,
        INotificationService notificationService,
        INavigationService navigationService)
    {
        _authTokenService = authTokenService;
        _notificationService = notificationService;
        _navigationService = navigationService;
    }
    
    public async Task LoadUserTokens(User user)
    {
        User = user;
        await RefreshTokensAsync();
    }
    
    private async Task RefreshTokensAsync()
    {
        if (User == null) return;
        
        Tokens.Clear();
        var tokens = await _authTokenService.GetUserTokensAsync(User.Id);
        
        foreach (var token in tokens)
        {
            Tokens.Add(token);
        }
    }
    
    [RelayCommand]
    private async Task DeactivateToken(UserAuthToken? token)
    {
        if (token == null)
        {
            _notificationService.ShowError("Выберите токен для деактивации");
            return;
        }
        
        await _authTokenService.DeactivateTokenAsync(token.Id);
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