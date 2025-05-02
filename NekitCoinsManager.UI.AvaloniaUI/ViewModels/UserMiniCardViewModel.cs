using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using MapsterMapper;
using NekitCoinsManager.Services;
using NekitCoinsManager.Shared.DTO;
using NekitCoinsManager.Shared.HttpClient;

namespace NekitCoinsManager.ViewModels;

public partial class UserMiniCardViewModel : ViewModelBase, ICurrentUserObserver, IMoneyOperationsObserverClient
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserBalanceServiceClient _userBalanceServiceClient;
    private readonly ITransactionServiceClient _transactionServiceClient;
    private readonly IMoneyOperationsServiceClient _moneyOperationsServiceClient;
    private readonly IMapper _mapper;
    
    [ObservableProperty]
    private UserDto? _currentUser;

    [ObservableProperty]
    private ObservableCollection<UserBalanceDto> _balances = new();

    public UserMiniCardViewModel(
        ICurrentUserService currentUserService,
        IUserBalanceServiceClient userBalanceServiceClient,
        ITransactionServiceClient transactionServiceClient,
        IMoneyOperationsServiceClient moneyOperationsServiceClient,
        IMapper mapper)
    {
        _currentUserService = currentUserService;
        _userBalanceServiceClient = userBalanceServiceClient;
        _transactionServiceClient = transactionServiceClient;
        _moneyOperationsServiceClient = moneyOperationsServiceClient;
        _mapper = mapper;
        
        // Подписываемся на обновления
        _currentUserService.Subscribe(this);
        _moneyOperationsServiceClient.Subscribe(this);
        
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
    
    // Обработчик для IMoneyOperationsObserverClient
    public void OnMoneyOperationsChanged()
    {
        LoadBalancesAsync();
    }
} 