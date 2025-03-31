using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MapsterMapper;
using NekitCoinsManager.Core.Models;
using NekitCoinsManager.Core.Services;
using NekitCoinsManager.Models;

namespace NekitCoinsManager.ViewModels;

public partial class TransactionDepositViewModel : ViewModelBase
{
    private readonly ITransactionService _transactionService;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationService _notificationService;
    private readonly ICurrencyService _currencyService;
    private readonly IMapper _mapper;

    /// <summary>
    /// Словарь валют: ключ - ID, значение - название валюты
    /// </summary>
    [ObservableProperty]
    private Dictionary<int, string> _currenciesDictionary = new();

    [ObservableProperty]
    private TransactionFormModel _transactionForm = new();

    [ObservableProperty]
    private User? _currentUser;

    public TransactionDepositViewModel(
        ITransactionService transactionService,
        ICurrentUserService currentUserService,
        INotificationService notificationService,
        ICurrencyService currencyService,
        IMapper mapper)
    {
        _transactionService = transactionService;
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
        LoadCurrenciesAsync();
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
            TransactionForm.ToUserId = CurrentUser.Id;
        }
    }

    /// <summary>
    /// Проверяет необходимые UI-условия перед отправкой формы
    /// (Основная валидация выполняется в TransactionService)
    /// </summary>
    private (bool isValid, string? errorMessage) ValidateFormUI()
    {
        if (CurrentUser == null)
            return (false, "Необходимо авторизоваться");

        // Базовая проверка заполненности полей формы, остальные проверки выполнит сервис
        if (TransactionForm.CurrencyId <= 0)
            return (false, "Выберите валюту для пополнения");
            
        if (TransactionForm.Amount <= 0)
            return (false, "Сумма пополнения должна быть больше нуля");

        return (true, null);
    }

    /// <summary>
    /// Очищает форму после успешной операции
    /// </summary>
    private void ResetForm()
    {
        TransactionForm = new TransactionFormModel
        {
            ToUserId = CurrentUser?.Id ?? 0
        };
    }

    [RelayCommand]
    private async Task Deposit()
    {
        var (isValid, errorMessage) = ValidateFormUI();
        if (!isValid)
        {
            _notificationService.ShowError(errorMessage ?? "Некорректные данные");
            return;
        }
        
        // Преобразуем UI-модель в модель данных для сервиса
        var transaction = _mapper.Map<Transaction>(TransactionForm);
        
        var (success, error) = await _transactionService.DepositCoinsAsync(transaction);
        
        if (!success)
        {
            _notificationService.ShowError(error ?? "Произошла ошибка при пополнении баланса");
            return;
        }

        // Очищаем форму после успешного пополнения
        ResetForm();
        _notificationService.ShowSuccess("Баланс успешно пополнен");
    }

    /// <summary>
    /// Обновляет данные для формы
    /// </summary>
    public void Refresh()
    {
        Initialize();
    }
} 