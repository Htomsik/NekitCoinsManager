using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using MapsterMapper;
using NekitCoinsManager.Core.Models;
using NekitCoinsManager.Core.Services;
using NekitCoinsManager.Models;
using NekitCoinsManager.Shared.DTO;

namespace NekitCoinsManager.ViewModels;

public partial class TransactionHistoryViewModel : ViewModelBase, ITransactionObserver
{
    private readonly ITransactionService _transactionService;
    
    private readonly IMapper _mapper;

    [ObservableProperty]
    private ObservableCollection<TransactionDisplayModel> _transactions = new();

    [ObservableProperty]
    private bool _showAllTransactions = true;

    [ObservableProperty]
    private UserDto? _firstUser;

    [ObservableProperty]
    private UserDto? _secondUser;

    public TransactionHistoryViewModel(ITransactionService transactionService, IMapper mapper)
    {
        _transactionService = transactionService;
        _mapper = mapper;
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
        
        var headTransactions = allTransactions.Where(t => t.ParentTransactionId == null);
        
        var transactionDisplayModels = _mapper.Map<List<TransactionDisplayModel>>(headTransactions)
            .OrderByDescending(t => t.CreatedAt);
        
        Transactions = new ObservableCollection<TransactionDisplayModel>(transactionDisplayModels);
    }

    private static bool IsUserInvolved(UserDto? user, Transaction transaction) =>
        user == null || transaction.FromUserId == user.Id || transaction.ToUserId == user.Id;

    public void OnTransactionsChanged() => LoadTransactionsAsync();

    partial void OnShowAllTransactionsChanged(bool value) => LoadTransactionsAsync();

    partial void OnFirstUserChanged(UserDto? value) => LoadTransactionsAsync();

    partial void OnSecondUserChanged(UserDto? value) => LoadTransactionsAsync();

    public void SetUsersForFiltering(UserDto? first, UserDto? second)
    {
        FirstUser = first;
        SecondUser = second;
    }
} 