using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NekitCoinsManager.Services;

namespace NekitCoinsManager.ViewModels;

public partial class UserRegistrationViewModel : ViewModelBase
{
    private readonly IUserService _userService;
    
    [ObservableProperty]
    private string _username = string.Empty;
    
    [ObservableProperty]
    private string _password = string.Empty;
    
    [ObservableProperty]
    private string _confirmPassword = string.Empty;
    
    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public UserRegistrationViewModel(IUserService userService)
    {
        _userService = userService;
    }

    [RelayCommand]
    private async Task Register()
    {
        var (success, error) = await _userService.AddUserAsync(Username, Password, ConfirmPassword);
        
        if (!success)
        {
            ErrorMessage = error ?? "Произошла ошибка при регистрации";
            return;
        }

        // Очищаем поля после успешной регистрации
        Username = string.Empty;
        Password = string.Empty;
        ConfirmPassword = string.Empty;
        ErrorMessage = string.Empty;
    }
} 