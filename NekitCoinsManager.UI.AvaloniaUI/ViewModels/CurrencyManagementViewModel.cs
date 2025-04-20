using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NekitCoinsManager.Services;
using NekitCoinsManager.Shared.DTO;
using NekitCoinsManager.Shared.HttpClient;

namespace NekitCoinsManager.ViewModels;

public partial class CurrencyManagementViewModel : ViewModelBase
{
    private readonly ICurrencyServiceClient _currencyServiceClient;
    private readonly INotificationService _notificationService;

    [ObservableProperty]
    private ObservableCollection<CurrencyDto> _currencies = new();

    [ObservableProperty]
    private string _newCurrencyName = string.Empty;

    [ObservableProperty]
    private string _newCurrencyCode = string.Empty;

    [ObservableProperty]
    private string _newCurrencySymbol = string.Empty;

    [ObservableProperty]
    private decimal _newCurrencyExchangeRate = 1.0m;

    [ObservableProperty]
    private bool _newCurrencyIsDefaultForNewUsers = false;

    [ObservableProperty]
    private decimal _newCurrencyDefaultAmount = 100.0m;

    [ObservableProperty]
    private CurrencyDto? _selectedCurrency;

    public CurrencyManagementViewModel(
        ICurrencyServiceClient currencyServiceClient,
        INotificationService notificationService)
    {
        _currencyServiceClient = currencyServiceClient;
        _notificationService = notificationService;
        LoadCurrenciesAsync().ConfigureAwait(false);
    }

    private async Task LoadCurrenciesAsync()
    {
        try
        {
            var currencies = await _currencyServiceClient.GetCurrenciesAsync();
            Currencies.Clear();
            foreach (var currency in currencies)
            {
                Currencies.Add(currency);
            }
            OnPropertyChanged(nameof(Currencies));
        }
        catch (Exception ex)
        {
            _notificationService.ShowError($"Ошибка при загрузке валют: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task AddCurrencyAsync()
    {
        var currency = new CurrencyDto
        {
            Name = NewCurrencyName,
            Code = NewCurrencyCode,
            Symbol = NewCurrencySymbol,
            ExchangeRate = NewCurrencyExchangeRate,
            IsDefaultForNewUsers = NewCurrencyIsDefaultForNewUsers,
            DefaultAmount = NewCurrencyDefaultAmount,
            IsActive = true,
            LastUpdateTime = DateTime.UtcNow
        };

        var result = await _currencyServiceClient.AddCurrencyAsync(currency);
        if (result.success)
        {
            _notificationService.ShowSuccess("Валюта успешно добавлена");
            await LoadCurrenciesAsync();
            ClearNewCurrencyFields();
        }
        else
        {
            _notificationService.ShowError(result.error ?? "Неизвестная ошибка");
        }
    }

    [RelayCommand]
    private async Task DeleteCurrencyAsync()
    {
        if (SelectedCurrency == null)
        {
            _notificationService.ShowError("Выберите валюту для удаления");
            return;
        }

        var result = await _currencyServiceClient.DeleteCurrencyAsync(SelectedCurrency.Id);
        if (result.success)
        {
            _notificationService.ShowSuccess("Валюта успешно удалена");
            await LoadCurrenciesAsync();
            SelectedCurrency = null;
        }
        else
        {
            _notificationService.ShowError(result.error ?? "Неизвестная ошибка");
        }
    }

    private void ClearNewCurrencyFields()
    {
        NewCurrencyName = string.Empty;
        NewCurrencyCode = string.Empty;
        NewCurrencySymbol = string.Empty;
        NewCurrencyExchangeRate = 1.0m;
        NewCurrencyIsDefaultForNewUsers = false;
        NewCurrencyDefaultAmount = 100.0m;
    }
} 