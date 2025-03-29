using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using NekitCoinsManager.Core.Models;
using NekitCoinsManager.Core.Services;

namespace NekitCoinsManager.ViewModels;

public partial class TransactionHistoryViewModel : ViewModelBase, ITransactionObserver
{
    private readonly ITransactionService _transactionService;

    [ObservableProperty]
    private ObservableCollection<Transaction> _transactions = new();

    [ObservableProperty]
    private bool _showAllTransactions = true;

    [ObservableProperty]
    private User? _firstUser;

    [ObservableProperty]
    private User? _secondUser;

    public TransactionHistoryViewModel(ITransactionService transactionService)
    {
        _transactionService = transactionService;
        _transactionService.Subscribe(this);
        LoadTransactions();
    }

    private void LoadTransactions()
    {
        var allTransactions = _transactionService.GetTransactions();
        
        if (!ShowAllTransactions && (FirstUser != null || SecondUser != null))
        {
            allTransactions = allTransactions.Where(t => 
                IsUserInvolved(FirstUser, t) && 
                (SecondUser == null || IsUserInvolved(SecondUser, t))
            );
        }

        Transactions = new ObservableCollection<Transaction>(allTransactions);
    }

    private static bool IsUserInvolved(User? user, Transaction transaction) =>
        user == null || transaction.FromUserId == user.Id || transaction.ToUserId == user.Id;

    public void OnTransactionsChanged() => LoadTransactions();

    partial void OnShowAllTransactionsChanged(bool value) => LoadTransactions();

    partial void OnFirstUserChanged(User? value) => LoadTransactions();

    partial void OnSecondUserChanged(User? value) => LoadTransactions();

    public void SetUsersForFiltering(User? first, User? second)
    {
        FirstUser = first;
        SecondUser = second;
    }
} 