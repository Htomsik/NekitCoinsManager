using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NekitCoinsManager.Services;

namespace NekitCoinsManager.ViewModels;

public partial class UserLoginViewModel : ViewModelBase
{
    private readonly IAuthService _authService;
    private readonly INotificationService _notificationService;
    
    [ObservableProperty]
    private string _username = string.Empty;
    
    [ObservableProperty]
    private string _password = string.Empty;
    
    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public UserLoginViewModel(IAuthService authService, INotificationService notificationService)
    {
        _authService = authService;
        _notificationService = notificationService;
    }

    [RelayCommand]
    private async Task Login()
    {
        var (success, error) = await _authService.LoginAsync(Username, Password);
        
        if (!success)
        {
            _notificationService.ShowError(error ?? "Произошла ошибка при входе");
            return;
        }

        // Очищаем поля после успешной авторизации
        Username = string.Empty;
        Password = string.Empty;
        _notificationService.ShowSuccess("Вход выполнен успешно");
    }
} 