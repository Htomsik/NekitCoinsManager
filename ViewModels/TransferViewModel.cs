using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NekitCoinsManager.Models;
using NekitCoinsManager.Services;

namespace NekitCoinsManager.ViewModels;

public partial class TransferViewModel : ViewModelBase, IUserObserver
{
    private readonly ITransactionService _transactionService;
    private readonly IUserService _userService;

    [ObservableProperty]
    private ObservableCollection<User> _users = new();

    [ObservableProperty]
    private Transaction _newTransaction = new();

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public TransferViewModel(ITransactionService transactionService, IUserService userService)
    {
        _transactionService = transactionService;
        _userService = userService;
        _userService.Subscribe(this);
        LoadUsers();
    }

    private void LoadUsers()
    {
        Users = new ObservableCollection<User>(_userService.GetUsers());
    }

    public void OnUsersChanged()
    {
        LoadUsers();
    }

    [RelayCommand]
    private async Task Transfer()
    {
        try
        {
            ErrorMessage = string.Empty;

            if (NewTransaction.FromUser == null || NewTransaction.ToUser == null)
            {
                ErrorMessage = "Выберите отправителя и получателя";
                return;
            }

            NewTransaction.FromUserId = NewTransaction.FromUser.Id;
            NewTransaction.ToUserId = NewTransaction.ToUser.Id;

            await _transactionService.TransferCoins(NewTransaction);

            // Очищаем поля после успешного перевода
            NewTransaction = new Transaction();

            // Обновляем список пользователей для отображения новых балансов
            LoadUsers();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }
} 