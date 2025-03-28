using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using NekitCoinsManager.Data;
using NekitCoinsManager.Models;
using NekitCoinsManager.Services;

namespace NekitCoinsManager.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
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

    // Свойства видимости для разных views
    [ObservableProperty]
    private bool _isRegistrationVisible = true;

    [ObservableProperty]
    private bool _isUserManagementVisible;

    [ObservableProperty]
    private bool _isTransferVisible;

    [ObservableProperty]
    private bool _isTransactionHistoryVisible;

    public MainWindowViewModel(AppDbContext dbContext)
    {
        var userService = new UserService(dbContext);
        var transactionService = new TransactionService(dbContext, userService);

        TransferViewModel = new TransferViewModel(transactionService, userService);
        TransactionHistoryViewModel = new TransactionHistoryViewModel(transactionService);
        UserManagementViewModel = new UserManagementViewModel(userService);
        UserRegistrationViewModel = new UserRegistrationViewModel(userService);

        // Устанавливаем начальную view
        CurrentView = UserRegistrationViewModel;
    }

    // Команды для навигации
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

    private void HideAllViews()
    {
        IsRegistrationVisible = false;
        IsUserManagementVisible = false;
        IsTransferVisible = false;
        IsTransactionHistoryVisible = false;
    }
}