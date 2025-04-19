using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MapsterMapper;
using NekitCoinsManager.Models;
using NekitCoinsManager.Services;
using NekitCoinsManager.Shared.DTO.Operations;
using NekitCoinsManager.Shared.HttpClient;

namespace NekitCoinsManager.ViewModels;

public partial class TransactionConversionViewModel : ViewModelBase
{
    private readonly IMoneyOperationsServiceClient _moneyOperationsServiceClient;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationService _notificationService;
    private readonly ICurrencyServiceClient _currencyServiceClient;
    private readonly ICurrencyConversionServiceClient _currencyConversionServiceClient;
    private readonly IMapper _mapper;

    /// <summary>
    /// Модель отображения данных конвертации
    /// </summary>
    [ObservableProperty]
    private TransactionConversionDisplayModel _displayModel = new();

    /// <summary>
    /// Результат конвертации
    /// </summary>
    [ObservableProperty]
    private decimal _convertedAmount;

    public TransactionConversionViewModel(
        IMoneyOperationsServiceClient moneyOperationsServiceClient,
        ICurrentUserService currentUserService,
        INotificationService notificationService,
        ICurrencyServiceClient currencyServiceClient,
        ICurrencyConversionServiceClient currencyConversionServiceClient,
        IMapper mapper)
    {
        _moneyOperationsServiceClient = moneyOperationsServiceClient;
        _currentUserService = currentUserService;
        _notificationService = notificationService;
        _currencyServiceClient = currencyServiceClient;
        _currencyConversionServiceClient = currencyConversionServiceClient;
        _mapper = mapper;

        // Инициализируем данные
        Initialize();
        
        // Подписываемся на изменения свойств модели отображения
        DisplayModel.PropertyChanged += (sender, e) => {
            // Если изменилась сумма, валюты или другие свойства - пересчитываем конвертацию
            if (e.PropertyName == nameof(DisplayModel.Amount) || 
                e.PropertyName == nameof(DisplayModel.FromCurrency) ||
                e.PropertyName == nameof(DisplayModel.ToCurrency))
            {
                CalculateConversionAsync();
            }
        };
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
        
        // Создаем словарь валют
        var currenciesDictionary = currencies.ToDictionary(
            c => c.Id, 
            c => new CurrencyDisplayModel 
            { 
                Id = c.Id, 
                Name = c.Name, 
                Code = c.Code, 
                Symbol = c.Symbol 
            });
        
        // Устанавливаем в модель отображения
        DisplayModel.CurrenciesDictionary = currenciesDictionary;
        
        // Если есть валюты, выберем первую по умолчанию для поля FromCurrency и вторую для ToCurrency
        if (currenciesDictionary.Any())
        {
            var firstCurrency = currenciesDictionary.First().Value;
            var secondCurrency = currenciesDictionary.Count > 1 
                ? currenciesDictionary.Skip(1).First().Value 
                : firstCurrency;
                
            DisplayModel.FromCurrency = firstCurrency;
            DisplayModel.ToCurrency = secondCurrency;
        }
    }

    private void LoadCurrentUser()
    {
        var currentUser = _currentUserService.CurrentUser;
        if (currentUser != null)
        {
            DisplayModel.UserId = currentUser.Id;
        }
    }

    /// <summary>
    /// Очищает форму после успешной операции
    /// </summary>
    private void ResetForm()
    {
        DisplayModel.Reset();
        ConvertedAmount = 0;
    }

    /// <summary>
    /// Рассчитывает предварительную сумму конвертации без выполнения транзакции
    /// </summary>
    private async void CalculateConversionAsync()
    {
        // Проверяем валидность данных модели
        var (isValid, _) = DisplayModel.Validate();
        if (!isValid)
        {
            ConvertedAmount = 0;
            return;
        }
        
        try
        {
            // Получаем предварительный расчет
            decimal convertedAmount = await _currencyConversionServiceClient.ConvertAsync(
                DisplayModel.Amount, 
                DisplayModel.FromCurrency.Code, 
                DisplayModel.ToCurrency.Code);
                
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
        // Проверяем валидность данных модели
        var (isValid, errorMessage) = DisplayModel.Validate();
        if (!isValid)
        {
            _notificationService.ShowError(errorMessage ?? "Проверьте данные");
            return;
        }
        
        try
        {
            // Используем маппер для создания ConversionDto
            var conversionDto = _mapper.Map<ConversionDto>(DisplayModel);
            
            // Используем MoneyOperationsServiceClient для конвертации
            var result = await _moneyOperationsServiceClient.ConvertAsync(conversionDto);
                
            if (!result.Success)
            {
                _notificationService.ShowError(result.Error ?? "Произошла ошибка при конвертации");
                return;
            }
            
            // Получаем сконвертированную сумму из результата операции, если она доступна
            // TODO подумать как от этого избавиться
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
                message += $". Получено: {convertedAmount.Value} {DisplayModel.ToCurrency.Symbol}";
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