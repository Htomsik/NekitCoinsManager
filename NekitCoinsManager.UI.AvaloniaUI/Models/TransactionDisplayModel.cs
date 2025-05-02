using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using NekitCoinsManager.Shared.DTO;

namespace NekitCoinsManager.Models;

/// <summary>
/// Модель для отображения транзакции со связанными дочерними транзакциями
/// </summary>
public partial class TransactionDisplayModel : ObservableObject
{
    /// <summary>
    /// ID транзакции
    /// </summary>
    [ObservableProperty]
    private int _id;
    
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
    
    /// <summary>
    /// Дата создания транзакции
    /// </summary>
    [ObservableProperty]
    private DateTime _createdAt;
    
    /// <summary>
    /// Тип транзакции
    /// </summary>
    [ObservableProperty]
    private TransactionTypeDto _type = TransactionTypeDto.Transfer;
    
    /// <summary>
    /// Информация о пользователе-отправителе
    /// </summary>
    [ObservableProperty]
    private UserDto _fromUser = null!;
    
    /// <summary>
    /// Информация о пользователе-получателе
    /// </summary>
    [ObservableProperty]
    private UserDto _toUser = null!;
    
    /// <summary>
    /// Информация о валюте транзакции
    /// </summary>
    [ObservableProperty]
    private CurrencyDto _currency = null!;
    
    /// <summary>
    /// ID родительской транзакции (если текущая транзакция связана с другой)
    /// </summary>
    [ObservableProperty]
    private int? _parentTransactionId;
    
    /// <summary>
    /// Дочерние транзакции, связанные с текущей
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<TransactionDisplayModel> _childTransactions = new();
    
    /// <summary>
    /// Флаг, показывающий, имеет ли эта транзакция дочерние
    /// </summary>
    public bool HasChildTransactions => ChildTransactions.Count > 0;
} 