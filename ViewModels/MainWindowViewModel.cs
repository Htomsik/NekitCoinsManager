using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using NekitCoinsManager.Data;
using NekitCoinsManager.Models;
using NekitCoinsManager.Services;

namespace NekitCoinsManager.ViewModels;

public partial class MainWindowViewModel : ViewModelBase, IAuthObserver
{
    private readonly IAuthService _authService;

    [ObservableProperty]
    private UserLoginViewModel _loginViewModel;

    [ObservableProperty]
    private TransferViewModel _transferViewModel;

    [ObservableProperty]
    private TransactionHistoryViewModel _transactionHistoryViewModel;

    [ObservableProperty]
    private UserManagementViewModel _userManagementViewModel;

    [ObservableProperty]
    private UserRegistrationViewModel _userRegistrationViewModel;

    [ObservableProperty]
    private object _currentView;

    public MainWindowViewModel(
        IAuthService authService,
        UserLoginViewModel loginViewModel,
        TransferViewModel transferViewModel,
        TransactionHistoryViewModel transactionHistoryViewModel,
        UserManagementViewModel userManagementViewModel,
        UserRegistrationViewModel userRegistrationViewModel)
    {
        _authService = authService;
        _authService.Subscribe(this);

        LoginViewModel = loginViewModel;
        TransferViewModel = transferViewModel;
        TransactionHistoryViewModel = transactionHistoryViewModel;
        UserManagementViewModel = userManagementViewModel;
        UserRegistrationViewModel = userRegistrationViewModel;

        // Устанавливаем начальную view
        CurrentView = UserRegistrationViewModel;
    }

    public void OnAuthStateChanged(User? user)
    {
        // В будущем здесь может быть дополнительная логика
    }

    [RelayCommand]
    private void ShowLogin()
    {
        CurrentView = LoginViewModel;
    }

    [RelayCommand]
    private void ShowRegistration()
    {
        CurrentView = UserRegistrationViewModel;
    }

    [RelayCommand]
    private void ShowUserManagement()
    {
        CurrentView = UserManagementViewModel;
    }

    [RelayCommand]
    private void ShowTransfer()
    {
        CurrentView = TransferViewModel;
    }

    [RelayCommand]
    private void ShowTransactionHistory()
    {
        CurrentView = TransactionHistoryViewModel;
    }

    [RelayCommand]
    private void Logout()
    {
        _authService.Logout();
    }
}