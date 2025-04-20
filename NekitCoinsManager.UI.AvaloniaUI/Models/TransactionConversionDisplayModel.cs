using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using NekitCoinsManager.Models;

namespace NekitCoinsManager.Models;

/// <summary>
/// Модель для отображения данных конвертации валют в UI
/// </summary>
public partial class TransactionConversionDisplayModel : ObservableObject
{
    /// <summary>
    /// Сумма для конвертации
    /// </summary>
    [ObservableProperty]
    private decimal _amount;

    /// <summary>
    /// Исходная валюта
    /// </summary>
    [ObservableProperty]
    private CurrencyDisplayModel _fromCurrency = new();

    /// <summary>
    /// Целевая валюта
    /// </summary>
    [ObservableProperty]
    private CurrencyDisplayModel _toCurrency = new();
    
    /// <summary>
    /// Идентификатор пользователя, выполняющего конвертацию
    /// </summary>
    [ObservableProperty]
    private int _userId;
    
    /// <summary>
    /// Словарь валют для отображения: ключ - ID, значение - модель отображения валюты
    /// </summary>
    [ObservableProperty]
    private Dictionary<int, CurrencyDisplayModel> _currenciesDictionary = new();

    /// <summary>
    /// Проверяет валидность данных для конвертации
    /// </summary>
    /// <returns>Результат проверки (валидность, сообщение об ошибке)</returns>
    public (bool isValid, string? errorMessage) Validate()
    {
        if (UserId <= 0)
        {
            return (false, "Необходимо авторизоваться");
        }

        if (FromCurrency.Id <= 0)
        {
            return (false, "Выберите исходную валюту");
        }

        if (ToCurrency.Id <= 0)
        {
            return (false, "Выберите целевую валюту");
        }
            
        if (FromCurrency.Id == ToCurrency.Id)
        {
            return (false, "Выберите разные валюты для конвертации");
        }

        if (Amount <= 0)
        {
            return (false, "Сумма должна быть больше нуля");
        }

        return (true, null);
    }

    /// <summary>
    /// Очищает данные формы
    /// </summary>
    public void Reset()
    {
        Amount = 0;
    }
} 