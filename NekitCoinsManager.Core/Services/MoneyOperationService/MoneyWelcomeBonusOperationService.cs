using NekitCoinsManager.Core.Models;
using NekitCoinsManager.Core.Repositories;

namespace NekitCoinsManager.Core.Services;

/// <summary>
/// Операция выдачи приветственного бонуса новому пользователю
/// </summary>
public class MoneyWelcomeBonusOperationService : BaseMoneyOperationService<WelcomeBonusOperation>
{
    public MoneyWelcomeBonusOperationService(
        ITransactionService transactionService,
        IUserRepository userRepository,
        ICurrencyRepository currencyRepository,
        IUserBalanceService userBalanceService,
        IDbTransactionService dbTransactionService)
        : base(transactionService, userRepository, currencyRepository, userBalanceService, dbTransactionService)
    {
    }

    /// <summary>
    /// Выполняет операцию выдачи приветственного бонуса
    /// </summary>
    protected override async Task<MoneyOperationResult> ExecuteOperationAsync(WelcomeBonusOperation operationData)
    {
        // Получаем банковский аккаунт
        var bankAccount = await UserRepository.GetBankAccountAsync();
        if (bankAccount == null)
        {
            return MoneyOperationResult.CreateError("Банковский аккаунт не найден");
        }

        // Получаем валюты для начисления бонуса
        var currencies = await CurrencyRepository.GetWelcomeBonusCurrenciesAsync();
        if (!currencies.Any())
        {
            return MoneyOperationResult.CreateError("Нет доступных валют для начисления приветственного бонуса");
        }

        // Выполняем транзакции для каждой валюты
        foreach (var currency in currencies)
        {
            // Определяем количество начисляемой валюты
            decimal amount = currency.IsDefaultForNewUsers ? currency.DefaultAmount : 100;

            // Создаем транзакцию через базовый метод
            var transaction = await CreateTransactionAsync(new WelcomeBonusOperation
            {
                UserId = bankAccount.Id,
                NewUserId = operationData.NewUserId,
                CurrencyId = currency.Id,
                Amount = amount
            });

            // Проверяем баланс банковского аккаунта
            var (balanceValid, balanceError) = await UserBalanceService.ValidateUserBalanceAsync(
                transaction.FromUserId, transaction.CurrencyId, transaction.Amount);

            if (!balanceValid)
            {
                return MoneyOperationResult.CreateError(balanceError ?? "Недостаточно средств на балансе банковского аккаунта");
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
        }

        return MoneyOperationResult.CreateSuccess(0); // ID транзакции не важен для этой операции
    }

    /// <summary>
    /// Валидирует данные операции выдачи приветственного бонуса
    /// </summary>
    public override async Task<(bool isValid, string? errorMessage)> ValidateAsync(WelcomeBonusOperation operationData)
    {
        // Проверяем нового пользователя
        if (operationData.NewUserId <= 0)
        {
            return (false, "Необходимо указать ID нового пользователя");
        }

        // Проверяем, что пользователь существует
        var newUser = await UserRepository.GetByIdAsync(operationData.NewUserId);
        if (newUser == null)
        {
            return (false, "Указанный пользователь не найден");
        }

        // Проверяем, что пользователь не является банковским аккаунтом
        if (newUser.IsBankAccount)
        {
            return (false, "Нельзя выдать приветственный бонус банковскому аккаунту");
        }

        return (true, null);
    }

    /// <summary>
    /// Создает транзакцию из DTO
    /// </summary>
    protected override Task<Transaction> CreateTransactionAsync(WelcomeBonusOperation operationData)
    {
        var transaction = new Transaction
        {
            FromUserId = operationData.UserId,
            ToUserId = operationData.NewUserId,
            CurrencyId = operationData.CurrencyId,
            Amount = operationData.Amount,
            Comment = string.IsNullOrEmpty(operationData.Comment) 
                ? "Приветственный бонус для нового пользователя" 
                : operationData.Comment,
            CreatedAt = DateTime.UtcNow,
            Type = TransactionType.Transfer
        };
        
        return Task.FromResult(transaction);
    }
} 