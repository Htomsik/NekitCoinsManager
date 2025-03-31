using CommunityToolkit.Mvvm.ComponentModel;
using NekitCoinsManager.Core.Models;
using NekitCoinsManager.Core.Services;

namespace NekitCoinsManager.ViewModels;

public partial class TransactionViewModel : ViewModelBase
{
    private readonly ICurrentUserService _currentUserService;

    [ObservableProperty]
    private User? _currentUser;

    [ObservableProperty]
    private TransactionHistoryViewModel _transactionHistory;

    [ObservableProperty]
    private TransactionTransferViewModel _transactionTransfer;

    public TransactionViewModel(
        ICurrentUserService currentUserService,
        TransactionHistoryViewModel transactionHistory,
        TransactionTransferViewModel transactionTransfer)
    {
        _currentUserService = currentUserService;
        _transactionHistory = transactionHistory;
        _transactionTransfer = transactionTransfer;
        
        // Устанавливаем режим отображения только транзакций между пользователями
        _transactionHistory.ShowAllTransactions = false;
        
        LoadCurrentUser();
    }

    private void LoadCurrentUser()
    {
        CurrentUser = _currentUserService.CurrentUser;
        UpdateTransactionHistory();
    }

    private void UpdateTransactionHistory()
    {
        // При первичной загрузке получатель может быть не выбран
        TransactionHistory.SetUsersForFiltering(CurrentUser, null);
    }
} 