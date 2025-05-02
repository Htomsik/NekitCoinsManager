using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NekitCoinsManager.Models;
using NekitCoinsManager.Services;
using NekitCoinsManager.Shared.DTO;
using NekitCoinsManager.Shared.HttpClient;

namespace NekitCoinsManager.ViewModels;

public partial class UserRegistrationViewModel : ViewModelBase
{
    
    private readonly IUserAuthServiceClient _userAuthServiceClient;
    private readonly INotificationService _notificationService;
    private readonly IAuthService _authService;
    private readonly INavigationService _navigationService;
    
    [ObservableProperty]
    private string _username = string.Empty;
    
    [ObservableProperty]
    private string _password = string.Empty;
    
    [ObservableProperty]
    private string _confirmPassword = string.Empty;
    
    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public UserRegistrationViewModel(
        IUserAuthServiceClient userAuthServiceClient,
        INotificationService notificationService,
        IAuthService authService,
        INavigationService navigationService)
    {
        _userAuthServiceClient = userAuthServiceClient;
        _notificationService = notificationService;
        _authService = authService;
        _navigationService = navigationService;
    }

    [RelayCommand]
    private async Task Register()
    {
        var registrationDto = new UserAuthRegistrationDto
        {
            Username = Username,
            Password = Password,
            ConfirmPassword = ConfirmPassword
        };

        // Используем новый сервис аутентификации для регистрации
        var result = await _userAuthServiceClient.RegisterUserAsync(registrationDto);
        
        if (!result.success)
        {
            _notificationService.ShowError(result.error ?? "Произошла ошибка при регистрации");
            return;
        }

        // Автоматический вход после успешной регистрации
        var loginResult = await _authService.LoginAsync(Username, Password);
        if (loginResult.success)
        {
            _notificationService.ShowSuccess("Регистрация и вход выполнены успешно");
            // Перенаправляем на основную страницу
            _navigationService.NavigateTo(ViewType.TransactionTransfer);
        }
        else
        {
            // Регистрация успешна, но вход не выполнен
            _notificationService.ShowSuccess("Регистрация выполнена успешно. Пожалуйста, войдите в систему.");
            _navigationService.NavigateTo(ViewType.Login);
            
            // Очищаем поля формы
            Username = string.Empty;
            Password = string.Empty;
            ConfirmPassword = string.Empty;
        }
    }
}