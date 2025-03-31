using CommunityToolkit.Mvvm.ComponentModel;
using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Models;

/// <summary>
/// Модель формы для создания/редактирования транзакции в UI
/// </summary>
public partial class TransactionFormModel : ObservableObject
{
    /// <summary>
    /// ID пользователя-отправителя
    /// </summary>
    [ObservableProperty]
    private int _fromUserId;

    /// <summary>
    /// ID пользователя-получателя
    /// </summary>
    [ObservableProperty]
    private int _toUserId;

    /// <summary>
    /// ID валюты транзакции
    /// </summary>
    [ObservableProperty]
    private int _currencyId;

    /// <summary>
    /// Сумма транзакции
    /// </summary>
    [ObservableProperty]
    private decimal _amount;

    /// <summary>
    /// Комментарий к транзакции
    /// </summary>
    [ObservableProperty]
    private string _comment = string.Empty;
} 