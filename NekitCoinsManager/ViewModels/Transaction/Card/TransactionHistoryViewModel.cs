using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
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
        LoadTransactionsAsync();
    }

    private async void LoadTransactionsAsync()
    {
        var allTransactions = await _transactionService.GetTransactionsAsync();
        
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

    public void OnTransactionsChanged() => LoadTransactionsAsync();

    partial void OnShowAllTransactionsChanged(bool value) => LoadTransactionsAsync();

    partial void OnFirstUserChanged(User? value) => LoadTransactionsAsync();

    partial void OnSecondUserChanged(User? value) => LoadTransactionsAsync();

    public void SetUsersForFiltering(User? first, User? second)
    {
        FirstUser = first;
        SecondUser = second;
    }
} 