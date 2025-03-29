using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NekitCoinsManager.Core.Models;
using NekitCoinsManager.Core.Services;

namespace NekitCoinsManager.ViewModels;

public partial class TransactionViewModel : ViewModelBase
{
    private readonly ITransactionService _transactionService;
    private readonly IUserService _userService;
    private readonly ICurrentUserService _currentUserService;

    [ObservableProperty]
    private ObservableCollection<User> _users = new();

    [ObservableProperty]
    private Transaction _newTransaction = new();

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private User? _currentUser;

    [ObservableProperty]
    private TransactionHistoryViewModel _transactionHistory;

    [ObservableProperty]
    private User? _selectedRecipient;

    public TransactionViewModel(
        ITransactionService transactionService, 
        IUserService userService,
        ICurrentUserService currentUserService,
        TransactionHistoryViewModel transactionHistory)
    {
        _transactionService = transactionService;
        _userService = userService;
        _currentUserService = currentUserService;
        _transactionHistory = transactionHistory;
        
        // Устанавливаем режим отображения только транзакций между пользователями
        _transactionHistory.ShowAllTransactions = false;
        
        LoadUsers();
        LoadCurrentUser();
        
        // Инициализируем отправителя как текущего пользователя
        NewTransaction.FromUser = CurrentUser;
    }

    private void LoadUsers()
    {
        var allUsers = _userService.GetUsers();
        // Исключаем текущего пользователя из списка получателей
        Users = new ObservableCollection<User>(
            allUsers.Where(u => u.Id != CurrentUser?.Id)
        );
    }

    private void LoadCurrentUser()
    {
        CurrentUser = _currentUserService.CurrentUser;
        NewTransaction.FromUser = CurrentUser;
        UpdateTransactionHistory();
    }

    partial void OnSelectedRecipientChanged(User? value)
    {
        if (value != null) 
            NewTransaction.ToUser = value;
        UpdateTransactionHistory();
    }

    private void UpdateTransactionHistory()
    {
        TransactionHistory.SetUsersForFiltering(CurrentUser, SelectedRecipient);
    }

    [RelayCommand]
    private async Task Transfer()
    {
        if (CurrentUser == null)
        {
            ErrorMessage = "Необходимо авторизоваться";
            return;
        }

        // Устанавливаем текущего пользователя как отправителя
        NewTransaction.FromUser = CurrentUser;
        
        var (success, error) = await _transactionService.TransferCoinsAsync(NewTransaction);
        
        if (!success)
        {
            ErrorMessage = error ?? "Произошла ошибка при переводе";
            return;
        }

        // Очищаем поля после успешного перевода
        NewTransaction = new Transaction
        {
            FromUser = CurrentUser // Устанавливаем текущего пользователя для новой транзакции
        };
        SelectedRecipient = null;
        ErrorMessage = string.Empty;

        // Обновляем список пользователей для отображения новых балансов
        LoadUsers();
    }
} 