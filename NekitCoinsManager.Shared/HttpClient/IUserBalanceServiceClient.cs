using NekitCoinsManager.Shared.DTO;
using NekitCoinsManager.Shared.DTO.Operations;

namespace NekitCoinsManager.Shared.HttpClient;

/// <summary>
/// Интерфейс клиента сервиса управления балансами пользователей
/// </summary>
public interface IUserBalanceServiceClient
{
    /// <summary>
    /// Получает все балансы пользователя
    /// </summary>
    Task<IEnumerable<UserBalanceDto>> GetUserBalancesAsync(int userId);
    
    /// <summary>
    /// Получает баланс пользователя по указанной валюте
    /// </summary>
    Task<UserBalanceDto?> GetUserBalanceAsync(int userId, int currencyId);
    
    /// <summary>
    /// Обновляет баланс пользователя
    /// </summary>
    Task<OperationResultDto> UpdateBalanceAsync(int userId, int currencyId, decimal amount);
    
    /// <summary>
    /// Переводит средства между пользователями
    /// </summary>
    Task<OperationResultDto> TransferBalanceAsync(int fromUserId, int toUserId, int currencyId, decimal amount);
    
    /// <summary>
    /// Проверяет и создает баланс для пользователя, если его нет
    /// </summary>
    Task<OperationResultDto> EnsureUserHasBalanceAsync(int userId, int currencyId);
    
    /// <summary>
    /// Создает новый баланс для пользователя
    /// </summary>
    Task<OperationResultDto> CreateBalanceAsync(int userId, int currencyId, decimal amount);
    
    /// <summary>
    /// Получает существующий или создает новый баланс для пользователя
    /// </summary>
    Task<(OperationResultDto Result, UserBalanceDto? Balance)> GetOrCreateBalanceAsync(int userId, int currencyId, decimal initialAmount = 0);
    
    /// <summary>
    /// Проверяет, доступна ли указанная сумма на балансе пользователя
    /// </summary>
    Task<OperationResultDto> ValidateUserBalanceAsync(int userId, int currencyId, decimal amount);
    
    /// <summary>
    /// Переводит указанную сумму с одного баланса на другой
    /// </summary>
    Task<OperationResultDto> TransferAmountBetweenBalancesAsync(
        int fromUserId,
        int fromCurrencyId,
        int toUserId,
        int toCurrencyId,
        decimal amount, 
        decimal? amountToAdd = null);
} 