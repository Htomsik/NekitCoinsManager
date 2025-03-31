using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MapsterMapper;
using NekitCoinsManager.Core.Models;
using NekitCoinsManager.Core.Services;
using NekitCoinsManager.Models;

namespace NekitCoinsManager.ViewModels;

public partial class TransactionTransferViewModel : ViewModelBase
{
    private readonly ITransactionService _transactionService;
    private readonly IUserService _userService;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationService _notificationService;
    private readonly ICurrencyService _currencyService;
    private readonly IMapper _mapper;

    /// <summary>
    /// Словарь получателей: ключ - ID, значение - имя пользователя
    /// </summary>
    [ObservableProperty]
    private Dictionary<int, string> _usersDictionary = new();

    /// <summary>
    /// Словарь валют: ключ - ID, значение - название валюты
    /// </summary>
    [ObservableProperty]
    private Dictionary<int, string> _currenciesDictionary = new();

    [ObservableProperty]
    private TransactionFormModel _transactionForm = new();

    [ObservableProperty]
    private User? _currentUser;

    public TransactionTransferViewModel(
        ITransactionService transactionService, 
        IUserService userService,
        ICurrentUserService currentUserService,
        INotificationService notificationService,
        ICurrencyService currencyService,
        IMapper mapper)
    {
        _transactionService = transactionService;
        _userService = userService;
        _currentUserService = currentUserService;
        _notificationService = notificationService;
        _currencyService = currencyService;
        _mapper = mapper;

        // Инициализируем данные
        Initialize();
    }

    /// <summary>
    /// Инициализирует данные формы и загружает необходимые списки
    /// </summary>
    private void Initialize()
    {
        LoadCurrentUser();
        LoadUsersAsync();
        LoadCurrenciesAsync();
    }

    private async void LoadUsersAsync()
    {
        var allUsers = await _userService.GetUsersAsync();
        
        // Исключаем текущего пользователя из списка получателей и преобразуем в словарь
        UsersDictionary = allUsers
            .Where(u => u.Id != CurrentUser?.Id)
            .ToDictionary(u => u.Id, u => u.Username);
    }

    private async void LoadCurrenciesAsync()
    {
        var currencies = await _currencyService.GetCurrenciesAsync();
        
        // Преобразуем список валют в словарь
        CurrenciesDictionary = currencies.ToDictionary(c => c.Id, c => c.Name);
    }

    private void LoadCurrentUser()
    {
        CurrentUser = _currentUserService.CurrentUser;
        if (CurrentUser != null)
        {
            TransactionForm.FromUserId = CurrentUser.Id;
        }
    }

    /// <summary>
    /// Проверяет корректность заполнения формы
    /// </summary>
    private (bool isValid, string? errorMessage) ValidateForm()
    {
        if (CurrentUser == null)
            return (false, "Необходимо авторизоваться");

        if (TransactionForm.Amount <= 0)
            return (false, "Сумма должна быть больше нуля");

        if (TransactionForm.CurrencyId <= 0)
            return (false, "Выберите валюту для перевода");

        if (TransactionForm.ToUserId <= 0)
            return (false, "Выберите получателя");

        return (true, null);
    }

    /// <summary>
    /// Очищает форму после успешной операции
    /// </summary>
    private void ResetForm()
    {
        TransactionForm = new TransactionFormModel
        {
            FromUserId = CurrentUser?.Id ?? 0
        };
    }

    [RelayCommand]
    private async Task Transfer()
    {
        var (isValid, errorMessage) = ValidateForm();
        if (!isValid)
        {
            _notificationService.ShowError(errorMessage ?? "Некорректные данные");
            return;
        }
        
        // Преобразуем UI-модель в модель данных для сервиса
        var transaction = _mapper.Map<Transaction>(TransactionForm);
        
        var (success, error) = await _transactionService.TransferCoinsAsync(transaction);
        
        if (!success)
        {
            _notificationService.ShowError(error ?? "Произошла ошибка при переводе");
            return;
        }

        // Очищаем форму после успешного перевода
        ResetForm();
        _notificationService.ShowSuccess("Перевод выполнен успешно");
    }

    /// <summary>
    /// Обновляет данные для формы
    /// </summary>
    public void Refresh()
    {
        Initialize();
    }
}