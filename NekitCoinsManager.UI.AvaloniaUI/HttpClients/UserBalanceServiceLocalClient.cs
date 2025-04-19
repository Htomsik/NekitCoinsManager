using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MapsterMapper;
using NekitCoinsManager.Core.Services;
using NekitCoinsManager.Shared.DTO;
using NekitCoinsManager.Shared.DTO.Operations;
using NekitCoinsManager.Shared.HttpClient;

namespace NekitCoinsManager.HttpClients;

/// <summary>
/// Локальная реализация клиента сервиса управления балансами пользователей
/// </summary>
public class UserBalanceServiceLocalClient : IUserBalanceServiceClient
{
    private readonly IUserBalanceService _userBalanceService;
    private readonly IMapper _mapper;

    public UserBalanceServiceLocalClient(IUserBalanceService userBalanceService, IMapper mapper)
    {
        _userBalanceService = userBalanceService;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<UserBalanceDto>> GetUserBalancesAsync(int userId)
    {
        var balances = await _userBalanceService.GetUserBalancesAsync(userId);
        return balances.Select(b => _mapper.Map<UserBalanceDto>(b));
    }

    /// <inheritdoc />
    public async Task<UserBalanceDto?> GetUserBalanceAsync(int userId, int currencyId)
    {
        var balance = await _userBalanceService.GetUserBalanceAsync(userId, currencyId);
        return balance == null ? null : _mapper.Map<UserBalanceDto>(balance);
    }

    /// <inheritdoc />
    public async Task<(bool success, string? error)> UpdateBalanceAsync(int userId, int currencyId, decimal amount)
    {
        return await _userBalanceService.UpdateBalanceAsync(userId, currencyId, amount);
    }

    /// <inheritdoc />
    public async Task<(bool success, string? error)> TransferBalanceAsync(int fromUserId, int toUserId, int currencyId, decimal amount)
    {
        return await _userBalanceService.TransferBalanceAsync(fromUserId, toUserId, currencyId, amount);
    }

    /// <inheritdoc />
    public async Task<(bool success, string? error)> EnsureUserHasBalanceAsync(int userId, int currencyId)
    {
        return await _userBalanceService.EnsureUserHasBalanceAsync(userId, currencyId);
    }

    /// <inheritdoc />
    public async Task<(bool success, string? error)> CreateBalanceAsync(int userId, int currencyId, decimal amount)
    {
        return await _userBalanceService.CreateBalanceAsync(userId, currencyId, amount);
    }

    /// <inheritdoc />
    public async Task<(bool success, string? error, UserBalanceDto? balance)> GetOrCreateBalanceAsync(int userId, int currencyId, decimal initialAmount = 0)
    {
        var (success, error, balance) = await _userBalanceService.GetOrCreateBalanceAsync(userId, currencyId, initialAmount);
        var balanceDto = balance != null ? _mapper.Map<UserBalanceDto>(balance) : null;
        
        return (success, error, balanceDto);
    }

    /// <inheritdoc />
    public async Task<(bool isValid, string? errorMessage)> ValidateUserBalanceAsync(int userId, int currencyId, decimal amount)
    {
        return await _userBalanceService.ValidateUserBalanceAsync(userId, currencyId, amount);
    }

    /// <inheritdoc />
    public async Task<(bool success, string? error)> TransferAmountBetweenBalancesAsync(
        int fromUserId, 
        int fromCurrencyId, 
        int toUserId, 
        int toCurrencyId, 
        decimal amount, 
        decimal? amountToAdd = null)
    {
        return await _userBalanceService.TransferAmountBetweenBalancesAsync(
            fromUserId, 
            fromCurrencyId, 
            toUserId, 
            toCurrencyId, 
            amount, 
            amountToAdd);
    }
} 