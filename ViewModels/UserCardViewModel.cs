using CommunityToolkit.Mvvm.ComponentModel;
using NekitCoinsManager.Core.Models;
using NekitCoinsManager.Core.Services;

namespace NekitCoinsManager.ViewModels;

public partial class UserCardViewModel : ViewModelBase, ICurrentUserObserver
{
    private readonly ICurrentUserService _currentUserService;
    
    [ObservableProperty]
    private User? _currentUser;
    
    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public UserCardViewModel(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
        _currentUserService.Subscribe(this);
        LoadCurrentUser();
    }

    private void LoadCurrentUser()
    {
        CurrentUser = _currentUserService.CurrentUser;
    }

    public void OnCurrentUserChanged() => LoadCurrentUser();
} 