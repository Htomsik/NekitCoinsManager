using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using NekitCoinsManager.Data;
using NekitCoinsManager.Models;
using NekitCoinsManager.Services;

namespace NekitCoinsManager.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private TransferViewModel _transferViewModel;

    [ObservableProperty]
    private TransactionHistoryViewModel _transactionHistoryViewModel;

    [ObservableProperty]
    private UserManagementViewModel _userManagementViewModel;

    public MainWindowViewModel(AppDbContext dbContext)
    {
        var userService = new UserService(dbContext);
        var transactionService = new TransactionService(dbContext);

        TransferViewModel = new TransferViewModel(transactionService, userService);
        TransactionHistoryViewModel = new TransactionHistoryViewModel(transactionService);
        UserManagementViewModel = new UserManagementViewModel(userService);
    }
}