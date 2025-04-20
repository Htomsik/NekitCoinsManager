using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NekitCoinsManager.Models;
using NekitCoinsManager.Services;

namespace NekitCoinsManager.ViewModels;

public partial class MainWindowViewModel : ViewModelBase, ICurrentUserObserver, INavigationObserver
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
        _navigationService.Subscribe(this);
        
        // Текущее представление будет установлено через событие CurrentViewChanged
        
        _currentUserService.Subscribe(this);
        
        // Пытаемся восстановить сессию
        TryRestoreSessionAsync();
    }

    
    private async void TryRestoreSessionAsync()
    {
        var result = await _authService.TryRestoreSessionAsync();
        if (!result.success)
        {
            // Если не удалось восстановить сессию, показываем форму входа
            _navigationService.NavigateTo(ViewType.Login);
            
            // Если есть конкретная ошибка и это не просто отсутствие сохраненной сессии,
            // можно отобразить ее пользователю
            if (!string.IsNullOrEmpty(result.error) && 
                !result.error.Contains("Отсутствует сохраненная сессия") && 
                !result.error.Contains("Токен авторизации не найден"))
            {
                // Можно добавить отображение ошибки, если у вас есть сервис уведомлений
                // _notificationService.ShowError($"Ошибка восстановления сессии: {result.error}");
            }
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
        _navigationService.NavigateTo(_currentUserService.CurrentUser != null ? ViewType.TransactionTransfer : ViewType.Login);
        
        // Уведомляем UI об изменении состояния авторизации
        OnPropertyChanged(nameof(IsAuthenticated));
    }
    
    public void OnViewChanged(IViewModel value)
    {
        CurrentView = value;
    }
}