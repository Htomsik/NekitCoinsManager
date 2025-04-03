using NekitCoinsManager.Core.Models;
using NekitCoinsManager.Core.Repositories;

namespace NekitCoinsManager.Core.Services;

/// <summary>
/// Операция пополнения баланса пользователя
/// </summary>
public class MoneyDepositOperationService : BaseMoneyOperationService<DepositDto>
{
    public MoneyDepositOperationService(
        ITransactionService transactionService,
        IUserRepository userRepository,
        ICurrencyRepository currencyRepository,
        IUserBalanceService userBalanceService,
        IDbTransactionService dbTransactionService)
        : base(transactionService, userRepository, currencyRepository, userBalanceService, dbTransactionService)
    {
    }

    /// <summary>
    /// Выполняет операцию пополнения баланса
    /// </summary>
    protected override async Task<MoneyOperationResult> ExecuteOperationAsync(DepositDto operationData)
    {
        // Создаем транзакцию из DTO
        var transaction = await CreateTransactionAsync(operationData);
        
        // Получаем или создаем баланс пользователя
        var (balanceSuccess, balanceError, balance) = await UserBalanceService.GetOrCreateBalanceAsync(
            transaction.ToUserId, transaction.CurrencyId);
            
        if (!balanceSuccess || balance == null)
        {
            return MoneyOperationResult.CreateError(balanceError ?? "Не удалось получить или создать баланс пользователя");
        }

        // Увеличиваем баланс пользователя через UserBalanceService
        decimal newAmount = balance.Amount + transaction.Amount;
        var (updateSuccess, updateError) = await UserBalanceService.UpdateBalanceAsync(
            transaction.ToUserId, 
            transaction.CurrencyId, 
            newAmount);
            
        if (!updateSuccess)
        {
            return MoneyOperationResult.CreateError(updateError ?? "Ошибка при обновлении баланса пользователя");
        }
        
        // Сохраняем транзакцию
        var (addSuccess, addError) = await TransactionService.AddTransactionAsync(transaction);
        if (!addSuccess)
        {
            return MoneyOperationResult.CreateError(addError ?? "Ошибка при сохранении транзакции");
        }
        
        // Если все операции выполнены успешно
        return MoneyOperationResult.CreateSuccess(transaction.Id);
    }

    /// <summary>
    /// Валидирует данные операции пополнения
    /// </summary>
    public override async Task<(bool isValid, string? errorMessage)> ValidateAsync(DepositDto operationData)
    {
        // Выполняем базовую валидацию
        var (baseIsValid, baseErrorMessage) = await base.ValidateAsync(operationData);
        if (!baseIsValid)
        {
            return (false, baseErrorMessage);
        }

        // Дополнительная валидация для пополнения может быть добавлена здесь
        
        return (true, null);
    }

    /// <summary>
    /// Создает транзакцию из DTO
    /// </summary>
    protected override Task<Transaction> CreateTransactionAsync(DepositDto operationData)
    {
        // Получаем банковский аккаунт для указания отправителя (или используем пользователя как отправителя)
        // В данном примере используем того же пользователя как отправителя
        var transaction = new Transaction
        {
            FromUserId = operationData.UserId, // Отправитель - тот же пользователь
            ToUserId = operationData.UserId,   // Получатель - пользователь
            CurrencyId = operationData.CurrencyId,
            Amount = operationData.Amount,
            Comment = string.IsNullOrEmpty(operationData.Comment) 
                ? "Пополнение баланса" 
                : operationData.Comment,
            CreatedAt = DateTime.UtcNow,
            Type = TransactionType.Deposit
        };
        
        return Task.FromResult(transaction);
    }
} 