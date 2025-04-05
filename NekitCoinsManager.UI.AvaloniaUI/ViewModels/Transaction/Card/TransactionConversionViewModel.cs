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

namespace NekitCoinsManager.ViewModels;

public partial class TransactionConversionViewModel : ViewModelBase
{
    private readonly IMoneyOperationsManager _moneyOperationsManager;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationService _notificationService;
    private readonly ICurrencyService _currencyService;
    private readonly ICurrencyConversionService _currencyConversionService;
    private readonly IMapper _mapper;

    /// <summary>
    /// Словарь валют: ключ - ID, значение - модель отображения валюты
    /// </summary>
    [ObservableProperty]
    private Dictionary<int, CurrencyDisplayModel> _currenciesDictionary = new();

    [ObservableProperty]
    private string _amount = string.Empty;

    [ObservableProperty]
    private int _fromCurrencyId;

    [ObservableProperty]
    private int _toCurrencyId;

    [ObservableProperty]
    private decimal _convertedAmount;

    [ObservableProperty]
    private User? _currentUser;

    /// <summary>
    /// Символ валюты назначения
    /// </summary>
    public string TargetCurrencySymbol => 
        CurrenciesDictionary.TryGetValue(ToCurrencyId, out var currency) ? currency.Symbol : string.Empty;
        
    // Обновляем символ валюты при изменении выбранной валюты и запускаем расчет
    partial void OnToCurrencyIdChanged(int value)
    {
        OnPropertyChanged(nameof(TargetCurrencySymbol));
        CalculateConversionAsync();
    }
    
    // Запускаем расчет при изменении исходной валюты
    partial void OnFromCurrencyIdChanged(int value)
    {
        CalculateConversionAsync();
    }
    
    // Запускаем расчет при изменении суммы
    partial void OnAmountChanged(string value)
    {
        CalculateConversionAsync();
    }

    public TransactionConversionViewModel(
        IMoneyOperationsManager moneyOperationsManager,
        ICurrentUserService currentUserService,
        INotificationService notificationService,
        ICurrencyService currencyService,
        ICurrencyConversionService currencyConversionService,
        IMapper mapper)
    {
        _moneyOperationsManager = moneyOperationsManager;
        _currentUserService = currentUserService;
        _notificationService = notificationService;
        _currencyService = currencyService;
        _currencyConversionService = currencyConversionService;
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
        
        // Преобразуем список валют в словарь с дополнительной информацией
        CurrenciesDictionary = currencies.ToDictionary(
            c => c.Id, 
            c => new CurrencyDisplayModel 
            { 
                Id = c.Id, 
                Name = c.Name, 
                Code = c.Code, 
                Symbol = c.Symbol 
            });
        
        // Если есть валюты, выберем первую по умолчанию для обоих полей
        if (CurrenciesDictionary.Any())
        {
            var firstCurrency = CurrenciesDictionary.First().Key;
            var secondCurrency = CurrenciesDictionary.Count > 1 
                ? CurrenciesDictionary.Skip(1).First().Key 
                : firstCurrency;
                
            FromCurrencyId = firstCurrency;
            ToCurrencyId = secondCurrency;
        }
    }

    private void LoadCurrentUser()
    {
        CurrentUser = _currentUserService.CurrentUser;
    }

    /// <summary>
    /// Проверяет необходимые UI-условия перед отправкой формы
    /// </summary>
    private (bool isValid, string? errorMessage, decimal amount) ValidateFormUI(bool showErrors = false)
    {
        if (CurrentUser == null)
        {
            if (showErrors) _notificationService.ShowError("Необходимо авторизоваться");
            return (false, "Необходимо авторизоваться", 0);
        }

        if (FromCurrencyId <= 0)
        {
            if (showErrors) _notificationService.ShowError("Выберите исходную валюту");
            return (false, "Выберите исходную валюту", 0);
        }

        if (ToCurrencyId <= 0)
        {
            if (showErrors) _notificationService.ShowError("Выберите целевую валюту");
            return (false, "Выберите целевую валюту", 0);
        }
            
        if (FromCurrencyId == ToCurrencyId)
        {
            if (showErrors) _notificationService.ShowError("Выберите разные валюты для конвертации");
            return (false, "Выберите разные валюты для конвертации", 0);
        }

        if (string.IsNullOrEmpty(Amount))
        {
            return (false, "Введите сумму", 0);
        }

        if (!decimal.TryParse(Amount.Replace(',', '.'), out decimal amountValue) || amountValue <= 0)
        {
            if (showErrors) _notificationService.ShowError("Сумма должна быть больше нуля");
            return (false, "Сумма должна быть больше нуля", 0);
        }

        return (true, null, amountValue);
    }

    /// <summary>
    /// Очищает форму после успешной операции
    /// </summary>
    private void ResetForm()
    {
        Amount = string.Empty;
        ConvertedAmount = 0;
    }

    /// <summary>
    /// Рассчитывает предварительную сумму конвертации без выполнения транзакции
    /// </summary>
    private async void CalculateConversionAsync()
    {
        // Проверяем валидность формы без показа ошибок
        var (isValid, _, amountValue) = ValidateFormUI();
        if (!isValid)
        {
            ConvertedAmount = 0;
            return;
        }
        
        // Получаем валюты
        if (!CurrenciesDictionary.TryGetValue(FromCurrencyId, out var fromCurrency) ||
            !CurrenciesDictionary.TryGetValue(ToCurrencyId, out var toCurrency))
        {
            ConvertedAmount = 0;
            return;
        }
        
        try
        {
            // Получаем предварительный расчет
            decimal convertedAmount = await _currencyConversionService.ConvertAsync(
                amountValue, 
                fromCurrency.Code, 
                toCurrency.Code);
                
            ConvertedAmount = convertedAmount;
        }
        catch
        {
            // При ошибке просто сбрасываем результат без уведомления
            ConvertedAmount = 0;
        }
    }

    [RelayCommand]
    private async Task Convert()
    {
        // Проверяем валидность формы с показом ошибок
        var (isValid, _, amountValue) = ValidateFormUI(true);
        if (!isValid)
        {
            return;
        }
        
        try
        {
            // Создаем кортеж с параметрами для маппинга в ConversionDto
            var conversionParams = (
                userId: CurrentUser!.Id,
                fromCurrencyId: FromCurrencyId,
                toCurrencyId: ToCurrencyId,
                amount: amountValue
            );
            
            // Используем маппер для создания ConversionDto
            var conversionDto = _mapper.Map<ConversionDto>(conversionParams);
            
            // Используем MoneyOperationsManager для конвертации
            var result = await _moneyOperationsManager.ConvertAsync(conversionDto);
                
            if (!result.Success)
            {
                _notificationService.ShowError(result.Error ?? "Произошла ошибка при конвертации");
                return;
            }
            
            // Получаем сконвертированную сумму из результата операции, если она доступна
            decimal? convertedAmount = null;
            if (result.Data is decimal decimalAmount)
            {
                convertedAmount = decimalAmount;
            }
            else if (result.Data is object dataObj && dataObj.GetType().GetProperty("ConvertedAmount")?.GetValue(dataObj) is decimal convertedDecimal)
            {
                convertedAmount = convertedDecimal;
            }

            // Очищаем форму после успешной конвертации
            ResetForm();
            
            // Формируем сообщение об успешной конвертации
            string message = "Конвертация выполнена успешно";
            if (convertedAmount.HasValue)
            {
                message += $". Получено: {convertedAmount.Value} {CurrenciesDictionary[ToCurrencyId].Symbol}";
            }
            
            _notificationService.ShowSuccess(message);
        }
        catch (System.Exception)
        {
            _notificationService.ShowError("Произошла ошибка при конвертации валют");
        }
    }

    /// <summary>
    /// Обновляет данные для формы
    /// </summary>
    public void Refresh()
    {
        Initialize();
    }
} 