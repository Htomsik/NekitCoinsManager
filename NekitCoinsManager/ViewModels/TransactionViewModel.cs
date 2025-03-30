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
    private readonly INotificationService _notificationService;
    private readonly ICurrencyService _currencyService;

    [ObservableProperty]
    private ObservableCollection<User> _users = new();

    [ObservableProperty]
    private ObservableCollection<Currency> _currencies = new();

    [ObservableProperty]
    private Transaction _newTransaction = new();

    [ObservableProperty]
    private User? _currentUser;

    [ObservableProperty]
    private TransactionHistoryViewModel _transactionHistory;

    [ObservableProperty]
    private User? _selectedRecipient;

    [ObservableProperty]
    private Currency? _selectedCurrency;

    public TransactionViewModel(
        ITransactionService transactionService, 
        IUserService userService,
        ICurrentUserService currentUserService,
        INotificationService notificationService,
        ICurrencyService currencyService,
        TransactionHistoryViewModel transactionHistory)
    {
        _transactionService = transactionService;
        _userService = userService;
        _currentUserService = currentUserService;
        _notificationService = notificationService;
        _currencyService = currencyService;
        _transactionHistory = transactionHistory;
        
        // Устанавливаем режим отображения только транзакций между пользователями
        _transactionHistory.ShowAllTransactions = false;
        
        LoadCurrentUser();
        LoadUsersAsync();
        LoadCurrenciesAsync();
    }

    private async void LoadUsersAsync()
    {
        var allUsers = await _userService.GetUsersAsync();
        // Исключаем текущего пользователя из списка получателей
        Users = new ObservableCollection<User>(
            allUsers.Where(u => u.Id != CurrentUser?.Id)
        );
    }

    private async void LoadCurrenciesAsync()
    {
        var currencies = await _currencyService.GetCurrenciesAsync();
        Currencies = new ObservableCollection<Currency>(currencies);
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

    partial void OnSelectedCurrencyChanged(Currency? value)
    {
        if (value != null)
            NewTransaction.Currency = value;
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
            _notificationService.ShowError("Необходимо авторизоваться");
            return;
        }

        if (NewTransaction.Amount <= 0)
        {
            _notificationService.ShowError("Сумма должна быть больше нуля");
            return;
        }

        if (NewTransaction.Currency == null)
        {
            _notificationService.ShowError("Выберите валюту для перевода");
            return;
        }

        var (success, error) = await _transactionService.TransferCoinsAsync(NewTransaction);
        
        if (!success)
        {
            _notificationService.ShowError(error ?? "Произошла ошибка при переводе");
            return;
        }

        // Очищаем поля после успешного перевода
        NewTransaction = new Transaction
        {
            FromUser = CurrentUser // Устанавливаем текущего пользователя для новой транзакции
        };
        SelectedRecipient = null;
        SelectedCurrency = null;
        _notificationService.ShowSuccess("Перевод выполнен успешно");

        // Обновляем список пользователей для отображения новых балансов
        LoadUsersAsync();
    }
} 