using NekitCoinsManager.Core.Models;
using NekitCoinsManager.Core.Repositories;

namespace NekitCoinsManager.Core.Services;

/// <summary>
/// Базовый абстрактный класс для операций с деньгами
/// </summary>
/// <typeparam name="TDto">Тип DTO для операции</typeparam>
public abstract class BaseMoneyOperationService<TDto> : IMoneyOperationService<TDto> where TDto : MoneyOperationDto
{
    protected readonly ITransactionService TransactionService;
    protected readonly IUserRepository UserRepository;
    protected readonly ICurrencyRepository CurrencyRepository;
    protected readonly IUserBalanceService UserBalanceService;
    protected readonly IDbTransactionService DbTransactionService;

    /// <summary>
    /// Конструктор базового класса денежных операций
    /// </summary>
    protected BaseMoneyOperationService(
        ITransactionService transactionService,
        IUserRepository userRepository,
        ICurrencyRepository currencyRepository,
        IUserBalanceService userBalanceService,
        IDbTransactionService dbTransactionService)
    {
        TransactionService = transactionService;
        UserRepository = userRepository;
        CurrencyRepository = currencyRepository;
        UserBalanceService = userBalanceService;
        DbTransactionService = dbTransactionService;
    }

    /// <summary>
    /// Выполняет денежную операцию
    /// </summary>
    public async Task<MoneyOperationResult> ExecuteAsync(TDto operationData)
    {
        // Сначала выполняем валидацию
        var (isValid, errorMessage) = await ValidateAsync(operationData);
        if (!isValid)
        {
            return MoneyOperationResult.CreateError(errorMessage ?? "Ошибка валидации данных");
        }

        // Выполняем операцию в транзакции
        var (success, operationResult) = await DbTransactionService.ExecuteInTransactionAsync(async () => 
        {
            // Выполняем конкретную операцию
            var result = await ExecuteOperationAsync(operationData);
            // Возвращаем результат в формате (bool success, MoneyOperationResult result)
            return (result.Success, result);
        });
        
        return operationResult;
    }
    
    /// <summary>
    /// Реализует конкретную операцию
    /// </summary>
    /// <param name="operationData">Данные для выполнения операции</param>
    /// <returns>Результат выполнения операции</returns>
    protected abstract Task<MoneyOperationResult> ExecuteOperationAsync(TDto operationData);
    
    /// <summary>
    /// Валидирует данные операции
    /// </summary>
    public virtual async Task<(bool isValid, string? errorMessage)> ValidateAsync(TDto operationData)
    {
        // Базовая валидация для всех операций
        if (operationData == null)
        {
            return (false, "Данные операции не могут быть пустыми");
        }

        if (operationData.Amount <= 0)
        {
            return (false, "Сумма операции должна быть больше нуля");
        }

        // Проверяем существование валюты
        var currency = await CurrencyRepository.GetByIdAsync(operationData.CurrencyId);
        if (currency == null)
        {
            return (false, "Указанная валюта не найдена");
        }

        // Проверяем существование пользователя
        var user = await UserRepository.GetByIdAsync(operationData.UserId);
        if (user == null)
        {
            return (false, "Указанный пользователь не найден");
        }

        return (true, null);
    }

    /// <summary>
    /// Создает транзакцию из DTO
    /// </summary>
    protected abstract Task<Transaction> CreateTransactionAsync(TDto operationData);
} 