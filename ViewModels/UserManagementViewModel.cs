using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NekitCoinsManager.Core.Models;
using NekitCoinsManager.Core.Services;

namespace NekitCoinsManager.ViewModels;

public partial class UserManagementViewModel : ViewModelBase, IUserObserver
{
    private readonly IUserService _userService;
    
    [ObservableProperty]
    private ObservableCollection<User> _users = new();
    
    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private User? _selectedUser;

    public UserManagementViewModel(IUserService userService)
    {
        _userService = userService;
        _userService.Subscribe(this);
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

    public void OnUsersChanged()
    {
        LoadUsers();
    }

    [RelayCommand]
    private async Task DeleteUser(User? user)
    {
        if (user == null)
        {
            ErrorMessage = "Выберите пользователя для удаления";
            return;
        }

        var (success, error) = await _userService.DeleteUserAsync(user.Id);
        
        if (!success)
        {
            ErrorMessage = error ?? "Произошла ошибка при удалении пользователя";
            return;
        }

        ErrorMessage = string.Empty;
    }
} 