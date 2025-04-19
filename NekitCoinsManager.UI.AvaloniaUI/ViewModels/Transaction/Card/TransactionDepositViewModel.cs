using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MapsterMapper;
using NekitCoinsManager.Core.Models;
using NekitCoinsManager.Core.Services;
using NekitCoinsManager.Models;
using NekitCoinsManager.Services;
using NekitCoinsManager.Shared.DTO;
using NekitCoinsManager.Shared.HttpClient;

namespace NekitCoinsManager.ViewModels;

public partial class TransactionDepositViewModel : ViewModelBase
{
    private readonly IMoneyOperationsManager _moneyOperationsManager;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationService _notificationService;
    private readonly ICurrencyServiceClient _currencyServiceClient;
    private readonly IMapper _mapper;

    /// <summary>
    /// Словарь валют: ключ - ID, значение - название валюты
    /// </summary>
    [ObservableProperty]
    private Dictionary<int, string> _currenciesDictionary = new();

    [ObservableProperty]
    private TransactionFormModel _transactionForm = new();

    [ObservableProperty]
    private UserDto? _currentUser;

    public TransactionDepositViewModel(
        IMoneyOperationsManager moneyOperationsManager,
        ICurrentUserService currentUserService,
        INotificationService notificationService,
        ICurrencyServiceClient currencyServiceClient,
        IMapper mapper)
    {
        _moneyOperationsManager = moneyOperationsManager;
        _currentUserService = currentUserService;
        _notificationService = notificationService;
        _currencyServiceClient = currencyServiceClient;
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
        var currencies = await _currencyServiceClient.GetCurrenciesAsync();
        
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
        
        // Используем маппер для создания DepositDto из TransactionFormModel
        var depositDto = _mapper.Map<DepositDto>(TransactionForm);
        
        // Используем MoneyOperationsManager
        var result = await _moneyOperationsManager.DepositAsync(depositDto);
        
        if (!result.Success)
        {
            _notificationService.ShowError(result.Error ?? "Произошла ошибка при пополнении баланса");
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