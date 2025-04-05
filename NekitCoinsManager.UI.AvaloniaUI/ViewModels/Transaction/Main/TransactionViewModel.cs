using CommunityToolkit.Mvvm.ComponentModel;
using NekitCoinsManager.Core.Models;
using NekitCoinsManager.Core.Services;

namespace NekitCoinsManager.ViewModels;

public abstract partial class TransactionViewModel : ViewModelBase
{
    private readonly ICurrentUserService _currentUserService;

    [ObservableProperty]
    private User? _currentUser;

    [ObservableProperty]
    private TransactionHistoryViewModel _transactionCardHistory;

    [ObservableProperty]
    private IViewModel _transactionCardViewModel;

    public TransactionViewModel(
        ICurrentUserService currentUserService,
        TransactionHistoryViewModel transactionCardHistory,
        IViewModel transactionCardViewModel)
    {
        _currentUserService = currentUserService;
        _transactionCardHistory = transactionCardHistory;
        _transactionCardViewModel = transactionCardViewModel;
        
        // Устанавливаем режим отображения только транзакций между пользователями
        _transactionCardHistory.ShowAllTransactions = false;
        
        LoadCurrentUser();
    }

    public void LoadCurrentUser()
    {
        CurrentUser = _currentUserService.CurrentUser;
        UpdateTransactionHistory();
    }

    public void UpdateTransactionHistory()
    {
        // При первичной загрузке получатель может быть не выбран
        TransactionCardHistory.SetUsersForFiltering(CurrentUser, null);
    }
} 