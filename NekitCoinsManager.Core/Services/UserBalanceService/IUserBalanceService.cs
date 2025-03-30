using System.Collections.Generic;
using System.Threading.Tasks;
using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Core.Services;

public interface IUserBalanceService
{
    Task<IEnumerable<UserBalance>> GetUserBalancesAsync(int userId);
    Task<UserBalance?> GetUserBalanceAsync(int userId, int currencyId);
    Task<(bool success, string? error)> UpdateBalanceAsync(int userId, int currencyId, decimal amount);
    Task<(bool success, string? error)> TransferBalanceAsync(int fromUserId, int toUserId, int currencyId, decimal amount);
    Task<(bool success, string? error)> EnsureUserHasBalanceAsync(int userId, int currencyId);
    Task<(bool success, string? error)> CreateBalanceAsync(int userId, int currencyId, decimal amount);
} 