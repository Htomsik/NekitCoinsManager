using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NekitCoinsManager.Core.Models;
using NekitCoinsManager.Core.Services;

namespace NekitCoinsManager.ViewModels;

public partial class CurrencyManagementViewModel : ViewModelBase
{
    private readonly ICurrencyService _currencyService;
    private readonly INotificationService _notificationService;

    [ObservableProperty]
    private ObservableCollection<Currency> _currencies = new();

    [ObservableProperty]
    private string _newCurrencyName = string.Empty;

    [ObservableProperty]
    private string _newCurrencyCode = string.Empty;

    [ObservableProperty]
    private string _newCurrencySymbol = string.Empty;

    [ObservableProperty]
    private decimal _newCurrencyExchangeRate = 1.0m;

    [ObservableProperty]
    private Currency? _selectedCurrency;

    public CurrencyManagementViewModel(
        ICurrencyService currencyService,
        INotificationService notificationService)
    {
        _currencyService = currencyService;
        _notificationService = notificationService;
        LoadCurrenciesAsync().ConfigureAwait(false);
    }

    private async Task LoadCurrenciesAsync()
    {
        try
        {
            var currencies = await _currencyService.GetCurrenciesAsync();
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
        var currency = new Currency
        {
            Name = NewCurrencyName,
            Code = NewCurrencyCode,
            Symbol = NewCurrencySymbol,
            ExchangeRate = NewCurrencyExchangeRate
        };

        var (success, error) = await _currencyService.AddCurrencyAsync(currency);
        if (success)
        {
            _notificationService.ShowSuccess("Валюта успешно добавлена");
            await LoadCurrenciesAsync();
            ClearNewCurrencyFields();
        }
        else
        {
            _notificationService.ShowError(error ?? "Неизвестная ошибка");
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

        var (success, error) = await _currencyService.DeleteCurrencyAsync(SelectedCurrency.Id);
        if (success)
        {
            _notificationService.ShowSuccess("Валюта успешно удалена");
            await LoadCurrenciesAsync();
            SelectedCurrency = null;
        }
        else
        {
            _notificationService.ShowError(error ?? "Неизвестная ошибка");
        }
    }

    private void ClearNewCurrencyFields()
    {
        NewCurrencyName = string.Empty;
        NewCurrencyCode = string.Empty;
        NewCurrencySymbol = string.Empty;
        NewCurrencyExchangeRate = 1.0m;
    }
} 