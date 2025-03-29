using Microsoft.Extensions.DependencyInjection;
using NekitCoinsManager.Core.Data;
using NekitCoinsManager.Core.Services;
using NekitCoinsManager.ViewModels;

namespace NekitCoinsManager;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        // Регистрируем сервисы как Singleton
        services.AddSingleton<IAuthService, AuthService>();
        services.AddSingleton<IUserService, UserService>();
        services.AddSingleton<ITransactionService, TransactionService>();
        services.AddSingleton<ICurrentUserService, CurrentUserService>();

        // Регистрируем ViewModels
        services.AddSingleton<MainWindowViewModel>();
        services.AddTransient<UserLoginViewModel>();
        services.AddTransient<TransactionViewModel>();
        services.AddTransient<TransactionHistoryViewModel>();
        services.AddTransient<UserManagementViewModel>();
        services.AddTransient<UserRegistrationViewModel>();
        services.AddTransient<UserCardViewModel>();

        // Регистрируем DbContext как Singleton
        services.AddSingleton<AppDbContext>();

        return services;
    }
} 