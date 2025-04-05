using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NekitCoinsManager.Services;
using NekitCoinsManager.Shared.HttpClient;

namespace NekitCoinsManager.ViewModels;

public partial class UserRegistrationViewModel : ViewModelBase
{
    private readonly IUserServiceClient _userServiceClient;
    private readonly INotificationService _notificationService;
    
    [ObservableProperty]
    private string _username = string.Empty;
    
    [ObservableProperty]
    private string _password = string.Empty;
    
    [ObservableProperty]
    private string _confirmPassword = string.Empty;
    
    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public UserRegistrationViewModel(IUserServiceClient userServiceClient, INotificationService notificationService)
    {
        _userServiceClient = userServiceClient;
        _notificationService = notificationService;
    }

    [RelayCommand]
    private async Task Register()
    {
        var result = await _userServiceClient.AddUserAsync(Username, Password, ConfirmPassword);
        
        if (!result.Success)
        {
            _notificationService.ShowError(result.Error ?? "Произошла ошибка при регистрации");
            return;
        }

        // Очищаем поля после успешной регистрации
        Username = string.Empty;
        Password = string.Empty;
        ConfirmPassword = string.Empty;
        _notificationService.ShowSuccess("Регистрация выполнена успешно");
    }
} 