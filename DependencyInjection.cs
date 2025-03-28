using Microsoft.Extensions.DependencyInjection;
using NekitCoinsManager.Data;
using NekitCoinsManager.Services;
using NekitCoinsManager.ViewModels;

namespace NekitCoinsManager;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        // DbContext
        services.AddSingleton<AppDbContext>();

        // Services
        services.AddSingleton<IUserService, UserService>();
        services.AddSingleton<ITransactionService, TransactionService>();

        // ViewModels
        services.AddSingleton<MainWindowViewModel>();
        services.AddTransient<UserRegistrationViewModel>();
        services.AddTransient<UserManagementViewModel>();
        services.AddTransient<TransferViewModel>();
        services.AddTransient<TransactionHistoryViewModel>();

        return services;
    }
} 