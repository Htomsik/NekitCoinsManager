using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NekitCoinsManager.Models;
using NekitCoinsManager.Services;

namespace NekitCoinsManager.ViewModels;

public partial class UserManagementViewModel : ViewModelBase, IUserObserver
{
    private readonly IUserService _userService;
    
    [ObservableProperty]
    private ObservableCollection<User> _users = new();
    
    [ObservableProperty]
    private string _newUsername = string.Empty;
    
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
    private async Task AddUser()
    {
        try
        {
            await _userService.AddUser(NewUsername);
            NewUsername = string.Empty;
            ErrorMessage = string.Empty;
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    [RelayCommand]
    private async Task DeleteUser(User? user)
    {
        if (user == null)
        {
            ErrorMessage = "Выберите пользователя для удаления";
            return;
        }

        try
        {
            await _userService.DeleteUser(user.Id);
            ErrorMessage = string.Empty;
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }
} 