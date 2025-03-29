using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NekitCoinsManager.Core.Models;
using NekitCoinsManager.Core.Services;

namespace NekitCoinsManager.ViewModels;

public partial class UserManagementViewModel : ViewModelBase
{
    private readonly IUserService _userService;
    private readonly INotificationService _notificationService;
    
    [ObservableProperty]
    private ObservableCollection<User> _users = new();
    
    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private User? _selectedUser;

    public UserManagementViewModel(
        IUserService userService,
        INotificationService notificationService)
    {
        _userService = userService;
        _notificationService = notificationService;
        LoadUsers();
    }

    private void LoadUsers()
    {
        Users.Clear();
        foreach (var user in _userService.GetUsers())
        {
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
} 