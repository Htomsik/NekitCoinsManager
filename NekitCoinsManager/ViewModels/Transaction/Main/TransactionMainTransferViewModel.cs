using NekitCoinsManager.Core.Services;

namespace NekitCoinsManager.ViewModels;

/// <summary>
/// Конкретная реализация TransactionViewModel для перевода между пользователями
/// </summary>
public class TransactionMainTransferViewModel : TransactionViewModel
{
    public TransactionMainTransferViewModel(
        ICurrentUserService currentUserService, 
        TransactionHistoryViewModel transactionCardHistory, 
        TransactionTransferViewModel transactionCardViewModel) 
        : base(currentUserService, transactionCardHistory, transactionCardViewModel)
    {
    }
} 