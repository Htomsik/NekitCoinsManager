using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Core.Repositories;

public interface IUserBalanceRepository : IRepository<UserBalance>
{
    Task<IEnumerable<UserBalance>> GetUserBalancesAsync(int userId);
    Task<UserBalance?> GetUserBalanceAsync(int userId, int currencyId);
    Task<bool> HasBalanceAsync(int userId, int currencyId);
    Task<bool> HasEnoughBalanceAsync(int userId, int currencyId, decimal amount);
    Task<IEnumerable<UserBalance>> GetBalancesByCurrencyAsync(int currencyId);
    Task<bool> HasBalancesWithCurrencyAsync(int currencyId);
} 