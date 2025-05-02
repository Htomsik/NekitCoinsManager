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
} 