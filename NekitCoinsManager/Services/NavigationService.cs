using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NekitCoinsManager.Core.Models;
using NekitCoinsManager.Core.Services;
using NekitCoinsManager.ViewModels;

namespace NekitCoinsManager.Services;



public class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;
    private ViewType _previousViewType = ViewType.TransactionTransfer;
    private readonly List<INavigationObserver> _observers = new();
    private IViewModel _currentView;
    public IViewModel CurrentView
    {
        get => _currentView;
        private set
        {
            if (_currentView == value) return;
            _currentView = value;
            NotifyObservers();
        }
    }

    public event EventHandler<IViewModel> CurrentViewChanged;

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void NavigateTo(ViewType viewType)
    {
        IViewModel viewModel = viewType switch
        {
            ViewType.Login => _serviceProvider.GetRequiredService<UserLoginViewModel>(),
            ViewType.Registration => _serviceProvider.GetRequiredService<UserRegistrationViewModel>(),
            ViewType.UserManagement => _serviceProvider.GetRequiredService<UserManagementViewModel>(),
            ViewType.TransactionTransfer => _serviceProvider.GetRequiredService<TransactionMainTransferViewModel>(),
            ViewType.TransactionDeposit => _serviceProvider.GetRequiredService<TransactionMainDepositViewModel>(),
            ViewType.TransactionConversion => _serviceProvider.GetRequiredService<TransactionMainConversionViewModel>(),
            ViewType.TransactionHistory => _serviceProvider.GetRequiredService<TransactionHistoryViewModel>(),
            ViewType.UserCard => _serviceProvider.GetRequiredService<UserCardViewModel>(),
            ViewType.CurrencyManagement => _serviceProvider.GetRequiredService<CurrencyManagementViewModel>(),
            ViewType.UserTokens => _serviceProvider.GetRequiredService<UserTokensViewModel>(),
            _ => throw new ArgumentException($"Unknown view type: {viewType}")
        };
        
        CurrentView = viewModel;
    }

    public async Task NavigateToWithParamsAsync<TParams>(ViewType viewType, TParams parameters, ViewType returnViewType)
    {
        // Запоминаем, откуда пришли, чтобы знать, куда вернуться
        _previousViewType = returnViewType;
        
        // Создаем вьюмодель для указанного типа представления
        var viewModel = viewType switch
        {
            ViewType.UserTokens => _serviceProvider.GetRequiredService<UserTokensViewModel>(),
            // Добавьте другие типы представлений здесь по мере необходимости
            _ => throw new NotSupportedException($"Навигация к {viewType} с параметрами типа {typeof(TParams).Name} не поддерживается")
        };
        
        // Инициализируем вьюмодель в соответствии с типом параметра
        switch (viewModel)
        {
            case UserTokensViewModel tokenViewModel when parameters is User user:
                await tokenViewModel.LoadUserTokens(user);
                break;
            // Добавьте другие типы вьюмоделей и параметров здесь по мере необходимости
            default:
                throw new NotSupportedException($"Инициализация вьюмодели {viewModel.GetType().Name} с параметрами типа {typeof(TParams).Name} не поддерживается");
        }
        
        // Устанавливаем текущее представление
        CurrentView = viewModel;
    }
    
    public void NavigateBack()
    {
        // Возвращаемся к предыдущему экрану
        NavigateTo(_previousViewType);
    }

    public void Subscribe(INavigationObserver observer)
    {
        _observers.Add(observer);
    }
    
    private void NotifyObservers()
    {
        foreach (var observer in _observers)
        {
            observer.OnViewChanged(CurrentView);
        }
    }
} 