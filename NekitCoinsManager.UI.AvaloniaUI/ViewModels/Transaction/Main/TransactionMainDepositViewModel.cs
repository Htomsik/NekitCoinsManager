using NekitCoinsManager.Core.Services;

namespace NekitCoinsManager.ViewModels;

/// <summary>
/// Конкретная реализация TransactionViewModel для пополнения баланса
/// </summary>
public class TransactionMainDepositViewModel : TransactionViewModel
{
    public TransactionMainDepositViewModel(
        ICurrentUserService currentUserService, 
        TransactionHistoryViewModel transactionCardHistory, 
        TransactionDepositViewModel transactionCardViewModel) 
        : base(currentUserService, transactionCardHistory, transactionCardViewModel)
    {
    }
} 