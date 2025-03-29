using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NekitCoinsManager.Models;
using NekitCoinsManager.Services;

namespace NekitCoinsManager.ViewModels;

public partial class TransactionViewModel : ViewModelBase, IUserObserver
{
    private readonly ITransactionService _transactionService;
    private readonly IUserService _userService;

    [ObservableProperty]
    private ObservableCollection<User> _users = new();

    [ObservableProperty]
    private Transaction _newTransaction = new();

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public TransactionViewModel(ITransactionService transactionService, IUserService userService)
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
        var (success, error) = await _transactionService.TransferCoinsAsync(NewTransaction);
        
        if (!success)
        {
            ErrorMessage = error ?? "Произошла ошибка при переводе";
            return;
        }

        // Очищаем поля после успешного перевода
        NewTransaction = new Transaction();
        ErrorMessage = string.Empty;

        // Обновляем список пользователей для отображения новых балансов
        LoadUsers();
    }
} 