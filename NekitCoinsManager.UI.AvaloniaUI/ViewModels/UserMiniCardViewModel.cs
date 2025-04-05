using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using NekitCoinsManager.Core.Models;
using NekitCoinsManager.Core.Services;
using NekitCoinsManager.Services;

namespace NekitCoinsManager.ViewModels;

public partial class UserMiniCardViewModel : ViewModelBase, ICurrentUserObserver, ITransactionObserver
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserBalanceService _userBalanceService;
    private readonly ITransactionService _transactionService;
    
    [ObservableProperty]
    private User? _currentUser;

    [ObservableProperty]
    private ObservableCollection<UserBalance> _balances = new();

    public UserMiniCardViewModel(
        ICurrentUserService currentUserService, 
        IUserBalanceService userBalanceService,
        ITransactionService transactionService)
    {
        _currentUserService = currentUserService;
        _userBalanceService = userBalanceService;
        _transactionService = transactionService;
        _currentUserService.Subscribe(this);
        _transactionService.Subscribe(this);
        LoadCurrentUser();
    }

    private void LoadCurrentUser()
    {
        CurrentUser = _currentUserService.CurrentUser;
        if (CurrentUser != null)
        {
            LoadBalancesAsync();
        }
        else
        {
            Balances.Clear();
        }
    }

    private async void LoadBalancesAsync()
    {
        if (CurrentUser == null) return;
        
        var balances = await _userBalanceService.GetUserBalancesAsync(CurrentUser.Id);
        Balances.Clear();
        foreach (var balance in balances)
        {
            Balances.Add(balance);
        }
    }

    public void OnCurrentUserChanged()
    {
        LoadCurrentUser();
    }

    public void OnTransactionsChanged()
    {
        LoadBalancesAsync();
    }
} 