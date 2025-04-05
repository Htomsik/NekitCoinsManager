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
    public async Task<OperationResultDto> UpdateBalanceAsync(int userId, int currencyId, decimal amount)
    {
        var (success, error) = await _userBalanceService.UpdateBalanceAsync(userId, currencyId, amount);
        return success 
            ? OperationResultDto.CreateSuccess() 
            : OperationResultDto.CreateError(error ?? "Не удалось обновить баланс");
    }

    /// <inheritdoc />
    public async Task<OperationResultDto> TransferBalanceAsync(int fromUserId, int toUserId, int currencyId, decimal amount)
    {
        var (success, error) = await _userBalanceService.TransferBalanceAsync(fromUserId, toUserId, currencyId, amount);
        return success
            ? OperationResultDto.CreateSuccess()
            : OperationResultDto.CreateError(error ?? "Не удалось выполнить перевод");
    }

    /// <inheritdoc />
    public async Task<OperationResultDto> EnsureUserHasBalanceAsync(int userId, int currencyId)
    {
        var (success, error) = await _userBalanceService.EnsureUserHasBalanceAsync(userId, currencyId);
        return success
            ? OperationResultDto.CreateSuccess()
            : OperationResultDto.CreateError(error ?? "Не удалось создать баланс");
    }

    /// <inheritdoc />
    public async Task<OperationResultDto> CreateBalanceAsync(int userId, int currencyId, decimal amount)
    {
        var (success, error) = await _userBalanceService.CreateBalanceAsync(userId, currencyId, amount);
        return success
            ? OperationResultDto.CreateSuccess()
            : OperationResultDto.CreateError(error ?? "Не удалось создать баланс");
    }

    /// <inheritdoc />
    public async Task<(OperationResultDto Result, UserBalanceDto? Balance)> GetOrCreateBalanceAsync(int userId, int currencyId, decimal initialAmount = 0)
    {
        var (success, error, balance) = await _userBalanceService.GetOrCreateBalanceAsync(userId, currencyId, initialAmount);
        
        if (!success)
        {
            return (OperationResultDto.CreateError(error ?? "Не удалось получить или создать баланс"), null);
        }
        
        var balanceDto = balance != null ? _mapper.Map<UserBalanceDto>(balance) : null;
        
        return (OperationResultDto.CreateSuccess(), balanceDto);
    }

    /// <inheritdoc />
    public async Task<OperationResultDto> ValidateUserBalanceAsync(int userId, int currencyId, decimal amount)
    {
        var (isValid, errorMessage) = await _userBalanceService.ValidateUserBalanceAsync(userId, currencyId, amount);
        return isValid
            ? OperationResultDto.CreateSuccess()
            : OperationResultDto.CreateError(errorMessage ?? "Недостаточно средств");
    }

    /// <inheritdoc />
    public async Task<OperationResultDto> TransferAmountBetweenBalancesAsync(
        int fromUserId, 
        int fromCurrencyId, 
        int toUserId, 
        int toCurrencyId, 
        decimal amount, 
        decimal? amountToAdd = null)
    {
        var (success, error) = await _userBalanceService.TransferAmountBetweenBalancesAsync(
            fromUserId, 
            fromCurrencyId, 
            toUserId, 
            toCurrencyId, 
            amount, 
            amountToAdd);
        
        return success
            ? OperationResultDto.CreateSuccess()
            : OperationResultDto.CreateError(error ?? "Не удалось выполнить перевод между балансами");
    }
} 