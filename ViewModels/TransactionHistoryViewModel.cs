using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using NekitCoinsManager.Core.Models;
using NekitCoinsManager.Core.Services;

namespace NekitCoinsManager.ViewModels;

public partial class TransactionHistoryViewModel : ViewModelBase, ITransactionObserver
{
    private readonly ITransactionService _transactionService;

    [ObservableProperty]
    private ObservableCollection<Transaction> _transactions = new();

    public TransactionHistoryViewModel(ITransactionService transactionService)
    {
        _transactionService = transactionService;
        _transactionService.Subscribe(this);
        LoadTransactions();
    }

    private void LoadTransactions()
    {
        Transactions = new ObservableCollection<Transaction>(_transactionService.GetTransactions());
    }

    public void OnTransactionsChanged()
    {
        LoadTransactions();
    }
} 