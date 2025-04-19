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

public partial class TransactionTransferViewModel : ViewModelBase
{
    private readonly IMoneyOperationsManager _moneyOperationsManager;
    private readonly IUserServiceClient _userServiceClient;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationService _notificationService;
    private readonly ICurrencyServiceClient _currencyServiceClient;
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
    private UserDto? _currentUser;

    public TransactionTransferViewModel(
        IMoneyOperationsManager moneyOperationsManager, 
        IUserServiceClient userServiceClient,
        ICurrentUserService currentUserService,
        INotificationService notificationService,
        ICurrencyServiceClient currencyServiceClient,
        IMapper mapper)
    {
        _moneyOperationsManager = moneyOperationsManager;
        _userServiceClient = userServiceClient;
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
        LoadUsersAsync();
        LoadCurrenciesAsync();
    }

    private async void LoadUsersAsync()
    {
        var allUserDtos = await _userServiceClient.GetUsersAsync();
        
        // Исключаем текущего пользователя из списка получателей и преобразуем в словарь
        UsersDictionary = allUserDtos
            .Where(u => u.Id != CurrentUser?.Id)
            .ToDictionary(u => u.Id, u => u.Username);
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
            TransactionForm.FromUserId = CurrentUser.Id;
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
        if (TransactionForm.ToUserId <= 0)
            return (false, "Выберите получателя");

        if (TransactionForm.CurrencyId <= 0)
            return (false, "Выберите валюту для перевода");

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
        var (isValid, errorMessage) = ValidateFormUI();
        if (!isValid)
        {
            _notificationService.ShowError(errorMessage ?? "Некорректные данные");
            return;
        }
        
        // Используем маппер для создания TransferDto из TransactionFormModel
        var transferDto = _mapper.Map<TransferDto>(TransactionForm);
        
        // Используем MoneyOperationsManager
        var result = await _moneyOperationsManager.TransferAsync(transferDto);
        
        if (!result.Success)
        {
            _notificationService.ShowError(result.Error ?? "Произошла ошибка при переводе");
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