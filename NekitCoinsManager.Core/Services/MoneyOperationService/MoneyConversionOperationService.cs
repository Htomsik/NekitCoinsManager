using NekitCoinsManager.Core.Models;
using NekitCoinsManager.Core.Repositories;

namespace NekitCoinsManager.Core.Services;

/// <summary>
/// Операция конвертации валюты
/// </summary>
public class MoneyConversionOperationService : BaseMoneyOperationService<ConversionDto>
{
    private readonly ICurrencyConversionService _currencyConversionService;

    public MoneyConversionOperationService(
        ITransactionService transactionService,
        IUserRepository userRepository,
        ICurrencyRepository currencyRepository,
        IUserBalanceService userBalanceService,
        IDbTransactionService dbTransactionService,
        ICurrencyConversionService currencyConversionService)
        : base(transactionService, userRepository, currencyRepository, userBalanceService, dbTransactionService)
    {
        _currencyConversionService = currencyConversionService;
    }

    /// <summary>
    /// Выполняет операцию конвертации валюты
    /// </summary>
    protected override async Task<MoneyOperationResult> ExecuteOperationAsync(ConversionDto operationData)
    {
        // Получаем валюты для конвертации
        var fromCurrency = await CurrencyRepository.GetByIdAsync(operationData.CurrencyId);
        var toCurrency = await CurrencyRepository.GetByIdAsync(operationData.TargetCurrencyId);
        
        if (fromCurrency == null || toCurrency == null)
        {
            return MoneyOperationResult.CreateError("Одна из валют не найдена");
        }
        
        // Получаем банковский аккаунт для записи транзакций
        var bankAccount = await UserRepository.GetBankAccountAsync();
        if (bankAccount == null)
        {
            return MoneyOperationResult.CreateError("Ошибка: банковский аккаунт не найден");
        }
        
        // Проверяем баланс пользователя для исходной валюты
        var (balanceValid, balanceError) = await UserBalanceService.ValidateUserBalanceAsync(
            operationData.UserId, operationData.CurrencyId, operationData.Amount);
            
        if (!balanceValid)
        {
            return MoneyOperationResult.CreateError(balanceError ?? "Недостаточно средств для конвертации");
        }
        
        // Конвертируем сумму с помощью сервиса конвертации
        decimal convertedAmount = await _currencyConversionService.ConvertAsync(
            operationData.Amount, 
            fromCurrency.Code, 
            toCurrency.Code);
        
        // Расчет комиссии из валюты
        decimal feePercentage = fromCurrency.ConversionFeePercentage;
        decimal feeAmount = operationData.Amount * feePercentage;
        
        // 1. Создаем транзакцию списания исходной валюты
        var withdrawTransaction = new Transaction
        {
            FromUserId = operationData.UserId,
            ToUserId = bankAccount.Id,
            CurrencyId = operationData.CurrencyId,
            Amount = operationData.Amount,
            Comment = $"Конвертация {operationData.Amount} {fromCurrency.Code} в {toCurrency.Code}",
            CreatedAt = DateTime.UtcNow,
            Type = TransactionType.Conversion
        };
        
        // Добавляем транзакцию списания
        var (withdrawSuccess, withdrawError) = await TransactionService.AddTransactionAsync(withdrawTransaction);
        if (!withdrawSuccess)
        {
            return MoneyOperationResult.CreateError(withdrawError ?? "Ошибка при сохранении транзакции списания");
        }
        
        // 2. Создаем транзакцию зачисления целевой валюты
        var depositTransaction = new Transaction
        {
            FromUserId = bankAccount.Id,
            ToUserId = operationData.UserId,
            CurrencyId = operationData.TargetCurrencyId,
            Amount = convertedAmount,
            Comment = $"Получено в результате конвертации {operationData.Amount} {fromCurrency.Code}",
            CreatedAt = DateTime.UtcNow,
            Type = TransactionType.Conversion,
            ParentTransactionId = withdrawTransaction.Id // Связь с транзакцией списания
        };
        
        // Добавляем транзакцию зачисления
        var (depositSuccess, depositError) = await TransactionService.AddTransactionAsync(depositTransaction);
        if (!depositSuccess)
        {
            return MoneyOperationResult.CreateError(depositError ?? "Ошибка при сохранении транзакции зачисления");
        }
        
        // 3. Создаем транзакцию комиссии, если она предусмотрена
        Transaction? feeTransaction = null;
        
        if (feeAmount > 0)
        {
            // Создаем транзакцию комиссии
            feeTransaction = new Transaction
            {
                FromUserId = operationData.UserId,
                ToUserId = bankAccount.Id,
                CurrencyId = operationData.CurrencyId,
                Amount = feeAmount,
                Comment = $"Комиссия за конвертацию {operationData.Amount} {fromCurrency.Code}",
                CreatedAt = DateTime.UtcNow,
                Type = TransactionType.Fee,
                ParentTransactionId = withdrawTransaction.Id // Связь с основной транзакцией
            };
            
            // Добавляем транзакцию комиссии
            var (feeTransactionSuccess, feeTransactionError) = await TransactionService.AddTransactionAsync(feeTransaction);
            if (!feeTransactionSuccess)
            {
                return MoneyOperationResult.CreateError(feeTransactionError ?? "Ошибка при сохранении транзакции комиссии");
            }
        }
        
        // После создания всех транзакций обновляем балансы
            
        // 1. Переводим средства с баланса пользователя на баланс банка в исходной валюте
        var (userToBankSuccess, userToBankError) = await UserBalanceService.TransferAmountBetweenBalancesAsync(
            operationData.UserId,
            operationData.CurrencyId,
            bankAccount.Id,
            operationData.CurrencyId,
            operationData.Amount,
            operationData.Amount);

        if (!userToBankSuccess)
        {
            return MoneyOperationResult.CreateError(userToBankError ?? "Ошибка при списании средств в исходной валюте");
        }

        // 2. Переводим средства с баланса банка на баланс пользователя в целевой валюте
        var (bankToUserSuccess, bankToUserError) = await UserBalanceService.TransferAmountBetweenBalancesAsync(
            bankAccount.Id,
            operationData.TargetCurrencyId,
            operationData.UserId,
            operationData.TargetCurrencyId,
            convertedAmount,
            convertedAmount);
        
        if (!bankToUserSuccess)
        {
            return MoneyOperationResult.CreateError(bankToUserError ?? "Ошибка при зачислении средств в целевой валюте");
        }
        
        // 3. Списываем комиссию с баланса пользователя, если она была создана
        if (feeTransaction != null)
        {
            var (feeSuccess, feeError) = await UserBalanceService.TransferAmountBetweenBalancesAsync(
                operationData.UserId,
                operationData.CurrencyId,
                bankAccount.Id,
                operationData.CurrencyId,
                feeAmount,
                feeAmount);
            
            if (!feeSuccess)
            {
                return MoneyOperationResult.CreateError(feeError ?? "Ошибка при списании комиссии");
            }
        }
        
        // Формируем успешный результат с конвертированной суммой
        var result = MoneyOperationResult.CreateSuccess(withdrawTransaction.Id);
        result.Data = new { ConvertedAmount = convertedAmount };
        
        return result;
    }

    /// <summary>
    /// Валидирует данные операции конвертации
    /// </summary>
    public override async Task<(bool isValid, string? errorMessage)> ValidateAsync(ConversionDto operationData)
    {
        // Выполняем базовую валидацию
        var (baseIsValid, baseErrorMessage) = await base.ValidateAsync(operationData);
        if (!baseIsValid)
        {
            return (false, baseErrorMessage);
        }

        // Проверяем целевую валюту
        if (operationData.TargetCurrencyId <= 0)
        {
            return (false, "Необходимо указать целевую валюту");
        }
        
        // Проверяем, что целевая валюта существует
        var targetCurrency = await CurrencyRepository.GetByIdAsync(operationData.TargetCurrencyId);
        if (targetCurrency == null)
        {
            return (false, "Указанная целевая валюта не найдена");
        }
        
        // Проверяем, что валюты различаются
        if (operationData.CurrencyId == operationData.TargetCurrencyId)
        {
            return (false, "Нельзя конвертировать валюту саму в себя");
        }
        
        // Дополнительная валидация, связанная с комиссией, уже не требуется, 
        // так как комиссия задается в модели валюты

        return (true, null);
    }

    /// <summary>
    /// Создает транзакцию из DTO
    /// </summary>
    protected override Task<Transaction> CreateTransactionAsync(ConversionDto operationData)
    {
        // Метод не используется напрямую, так как для конвертации создается несколько транзакций
        // Возвращаем null, так как реальные транзакции создаются в ExecuteOperationAsync
        return Task.FromResult<Transaction>(null);
    }
} 