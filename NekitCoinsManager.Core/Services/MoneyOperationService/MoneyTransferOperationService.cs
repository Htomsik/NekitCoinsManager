using NekitCoinsManager.Core.Models;
using NekitCoinsManager.Core.Repositories;

namespace NekitCoinsManager.Core.Services;

/// <summary>
/// Операция перевода денег между пользователями
/// </summary>
public class MoneyTransferOperationService : BaseMoneyOperationService<TransferDto>
{
    public MoneyTransferOperationService(
        ITransactionService transactionService,
        IUserRepository userRepository,
        ICurrencyRepository currencyRepository,
        IUserBalanceService userBalanceService,
        IDbTransactionService dbTransactionService)
        : base(transactionService, userRepository, currencyRepository, userBalanceService, dbTransactionService)
    {
    }

    /// <summary>
    /// Выполняет операцию перевода денег
    /// </summary>
    protected override async Task<MoneyOperationResult> ExecuteOperationAsync(TransferDto operationData)
    {
        // Создаем транзакцию из DTO
        var transaction = await CreateTransactionAsync(operationData);
        
        // Проверяем баланс пользователя через UserBalanceService
        var (balanceValid, balanceError) = await UserBalanceService.ValidateUserBalanceAsync(
            transaction.FromUserId, transaction.CurrencyId, transaction.Amount);
            
        if (!balanceValid)
        {
            return MoneyOperationResult.CreateError(balanceError ?? "Недостаточно средств на балансе");
        }
        
        // Выполняем перевод через UserBalanceService
        var (transferSuccess, transferError) = await UserBalanceService.TransferBalanceAsync(
            transaction.FromUserId, 
            transaction.ToUserId, 
            transaction.CurrencyId, 
            transaction.Amount
        );

        if (!transferSuccess)
        {
            return MoneyOperationResult.CreateError(transferError ?? "Ошибка при переводе средств");
        }

        // Добавляем транзакцию через TransactionService
        var (addSuccess, addError) = await TransactionService.AddTransactionAsync(transaction);
        if (!addSuccess)
        {
            return MoneyOperationResult.CreateError(addError ?? "Ошибка при сохранении транзакции");
        }

        // Если все операции выполнены успешно
        return MoneyOperationResult.CreateSuccess(transaction.Id);
    }

    /// <summary>
    /// Валидирует данные операции перевода
    /// </summary>
    public override async Task<(bool isValid, string? errorMessage)> ValidateAsync(TransferDto operationData)
    {
        // Выполняем базовую валидацию
        var (baseIsValid, baseErrorMessage) = await base.ValidateAsync(operationData);
        if (!baseIsValid)
        {
            return (false, baseErrorMessage);
        }

        // Проверяем получателя
        if (operationData.RecipientId <= 0)
        {
            return (false, "Необходимо указать получателя");
        }
        
        // Проверяем, что получатель существует
        var recipient = await UserRepository.GetByIdAsync(operationData.RecipientId);
        if (recipient == null)
        {
            return (false, "Указанный получатель не найден");
        }
        
        // Проверяем, что отправитель и получатель не один и тот же пользователь
        if (operationData.UserId == operationData.RecipientId)
        {
            return (false, "Нельзя отправить перевод самому себе");
        }

        return (true, null);
    }

    /// <summary>
    /// Создает транзакцию из DTO
    /// </summary>
    protected override Task<Transaction> CreateTransactionAsync(TransferDto operationData)
    {
        var transaction = new Transaction
        {
            FromUserId = operationData.UserId,
            ToUserId = operationData.RecipientId,
            CurrencyId = operationData.CurrencyId,
            Amount = operationData.Amount,
            Comment = string.IsNullOrEmpty(operationData.Comment) 
                ? "Перевод средств" 
                : operationData.Comment,
            CreatedAt = DateTime.UtcNow,
            Type = TransactionType.Transfer
        };
        
        return Task.FromResult(transaction);
    }
} 