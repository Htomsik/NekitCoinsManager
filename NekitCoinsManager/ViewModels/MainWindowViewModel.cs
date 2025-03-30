using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NekitCoinsManager.Core.Services;
using NekitCoinsManager.Services;

namespace NekitCoinsManager.ViewModels;

public partial class MainWindowViewModel : ViewModelBase, ICurrentUserObserver
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthService _authService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private IViewModel _currentView;

    [ObservableProperty]
    private UserMiniCardViewModel _userMiniCardViewModel;

    [ObservableProperty]
    private NotificationViewModel _notificationViewModel;

    public bool IsAuthenticated => _currentUserService.CurrentUser != null;

    public MainWindowViewModel(
        ICurrentUserService currentUserService,
        UserMiniCardViewModel userMiniCardViewModel,
        NotificationViewModel notificationViewModel,
        IAuthService authService,
        INavigationService navigationService)
    {
        _currentUserService = currentUserService;
        _userMiniCardViewModel = userMiniCardViewModel;
        _notificationViewModel = notificationViewModel;
        _authService = authService;
        _navigationService = navigationService;
        
        // Подписываемся на изменение текущего представления
        _navigationService.CurrentViewChanged += OnCurrentViewChanged;
        
        // Текущее представление будет установлено через событие CurrentViewChanged
        
        _currentUserService.Subscribe(this);
        
        // Пытаемся восстановить сессию
        TryRestoreSessionAsync();
    }
    
    private void OnCurrentViewChanged(object sender, IViewModel viewModel)
    {
        CurrentView = viewModel;
    }
    
    private async void TryRestoreSessionAsync()
    {
        var success = await _authService.TryRestoreSessionAsync();
        if (!success)
        {
            // Если не удалось восстановить сессию, показываем форму входа
            _navigationService.NavigateTo(ViewType.Login);
        }
    }
    
    [RelayCommand]
    private void Navigate(ViewType viewType)
    {
        _navigationService.NavigateTo(viewType);
    }
    
    public void OnCurrentUserChanged()
    {
        // Обновляем view в зависимости от состояния авторизации
        _navigationService.NavigateTo(_currentUserService.CurrentUser != null ? ViewType.Transaction : ViewType.Login);
        
        // Уведомляем UI об изменении состояния авторизации
        OnPropertyChanged(nameof(IsAuthenticated));
    }
}