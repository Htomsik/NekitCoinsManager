using NekitCoinsManager.Core.Services;

namespace NekitCoinsManager.ViewModels;

/// <summary>
/// Конкретная реализация TransactionViewModel для конвертации валюты
/// </summary>
public class TransactionMainConversionViewModel : TransactionViewModel
{
    public TransactionMainConversionViewModel(
        ICurrentUserService currentUserService, 
        TransactionHistoryViewModel transactionCardHistory, 
        TransactionConversionViewModel transactionCardViewModel) 
        : base(currentUserService, transactionCardHistory, transactionCardViewModel)
    {
    }
} 