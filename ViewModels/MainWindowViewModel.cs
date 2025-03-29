using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NekitCoinsManager.Core.Models;
using NekitCoinsManager.Core.Services;
using System.Collections.Generic;

namespace NekitCoinsManager.ViewModels;

public enum ViewType
{
    Login,
    Registration,
    UserManagement,
    Transaction,
    TransactionHistory,
    UserCard
}

public partial class MainWindowViewModel : ViewModelBase, IAuthObserver
{
    private readonly IAuthService _authService;
    private readonly Dictionary<ViewType, object> _viewModels;

    [ObservableProperty]
    private UserLoginViewModel _loginViewModel;

    [ObservableProperty]
    private TransactionViewModel _transactionViewModel;

    [ObservableProperty]
    private TransactionHistoryViewModel _transactionHistoryViewModel;

    [ObservableProperty]
    private UserManagementViewModel _userManagementViewModel;

    [ObservableProperty]
    private UserRegistrationViewModel _userRegistrationViewModel;

    [ObservableProperty]
    private UserCardViewModel _userCardViewModel;

    [ObservableProperty]
    private object _currentView;

    public MainWindowViewModel(
        IAuthService authService,
        UserLoginViewModel loginViewModel,
        TransactionViewModel transactionViewModel,
        TransactionHistoryViewModel transactionHistoryViewModel,
        UserManagementViewModel userManagementViewModel,
        UserRegistrationViewModel userRegistrationViewModel,
        UserCardViewModel userCardViewModel)
    {
        _authService = authService;
        _authService.Subscribe(this);

        LoginViewModel = loginViewModel;
        TransactionViewModel = transactionViewModel;
        TransactionHistoryViewModel = transactionHistoryViewModel;
        UserManagementViewModel = userManagementViewModel;
        UserRegistrationViewModel = userRegistrationViewModel;
        UserCardViewModel = userCardViewModel;

        _viewModels = new Dictionary<ViewType, object>
        {
            { ViewType.Login, LoginViewModel },
            { ViewType.Registration, UserRegistrationViewModel },
            { ViewType.UserManagement, UserManagementViewModel },
            { ViewType.Transaction, TransactionViewModel },
            { ViewType.TransactionHistory, TransactionHistoryViewModel },
            { ViewType.UserCard, UserCardViewModel }
        };

        // Устанавливаем начальную view
        NavigateTo(ViewType.Login);
    }

    public void OnAuthStateChanged(User? user)
    {
        // В будущем здесь может быть дополнительная логика
    }

    private void NavigateTo(ViewType viewType)
    {
        if (_viewModels.TryGetValue(viewType, out var view))
        {
            CurrentView = view;
        }
    }

    [RelayCommand]
    private void Navigate(ViewType viewType) => NavigateTo(viewType);

    [RelayCommand]
    private void Logout()
    {
        _authService.Logout();
    }
}