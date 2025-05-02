using System.Collections.Generic;
using System.Threading.Tasks;
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
} 