using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using MapsterMapper;
using NekitCoinsManager.Core.Models;
using NekitCoinsManager.Core.Services;
using NekitCoinsManager.Services;
using NekitCoinsManager.Shared.DTO;
using NekitCoinsManager.Shared.HttpClient;

namespace NekitCoinsManager.ViewModels;

public partial class UserMiniCardViewModel : ViewModelBase, ICurrentUserObserver, ITransactionObserver
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserBalanceServiceClient _userBalanceServiceClient;
    private readonly ITransactionService _transactionService;
    private readonly IMapper _mapper;
    
    [ObservableProperty]
    private UserDto? _currentUser;

    [ObservableProperty]
    private ObservableCollection<UserBalanceDto> _balances = new();

    public UserMiniCardViewModel(
        ICurrentUserService currentUserService,
        IUserBalanceServiceClient userBalanceServiceClient,
        ITransactionService transactionService,
        IMapper mapper)
    {
        _currentUserService = currentUserService;
        _userBalanceServiceClient = userBalanceServiceClient;
        _transactionService = transactionService;
        _mapper = mapper;
        _currentUserService.Subscribe(this);
        _transactionService.Subscribe(this);
        LoadCurrentUser();
        LoadBalancesAsync();
    }

    private void LoadCurrentUser()
    {
        CurrentUser = _currentUserService.CurrentUser;
        if (CurrentUser != null)
        {
            LoadBalancesAsync();
        }
        else
        {
            Balances.Clear();
        }
    }

    private async void LoadBalancesAsync()
    {
        if (CurrentUser == null) return;

        var balanceDtos = await _userBalanceServiceClient.GetUserBalancesAsync(CurrentUser.Id);
        Balances.Clear();
        
        foreach (var balanceDto in balanceDtos)
        {
            Balances.Add(balanceDto);
        }
    }

    public void OnCurrentUserChanged()
    {
        LoadCurrentUser();
    }

    public void OnTransactionsChanged()
    {
        LoadBalancesAsync();
    }
} 