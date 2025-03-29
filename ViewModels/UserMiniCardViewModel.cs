using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NekitCoinsManager.Core.Models;
using NekitCoinsManager.Core.Services;

namespace NekitCoinsManager.ViewModels;

public partial class UserMiniCardViewModel : ViewModelBase, ICurrentUserObserver
{
    private readonly ICurrentUserService _currentUserService;
    
    [ObservableProperty]
    private User? _currentUser;

    public UserMiniCardViewModel(ICurrentUserService currentUserService, ITransactionService transactionService)
    {
        _currentUserService = currentUserService;
        _currentUserService.Subscribe(this);
        LoadCurrentUser();
    }

    private void LoadCurrentUser()
    {
        CurrentUser = _currentUserService.CurrentUser;
    }

    public void OnCurrentUserChanged()
    {
        LoadCurrentUser();
    }
    
} 