using Microsoft.Extensions.DependencyInjection;
using NekitCoinsManager.Core.Data;
using NekitCoinsManager.Core.Services;
using NekitCoinsManager.Core.Services.AppSettingsService;
using NekitCoinsManager.ViewModels;

namespace NekitCoinsManager;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        // Регистрируем сервисы как Transient
        services.AddTransient<IAuthService, AuthService>();
        services.AddTransient<IAuthTokenService, AuthTokenService>();
        services.AddTransient<IHardwareInfoService, HardwareInfoService>();
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IPasswordHasherService, PasswordHasherService>();
        services.AddTransient<ICurrencyService, CurrencyService>();
        services.AddTransient<IUserBalanceService, UserBalanceService>();
        services.AddTransient<IUserSettingsService, UserFileSettingsService>();

        // Регистрируем сервисы как Singleton (требуют сохранения состояния)
        services.AddSingleton<IAppSettingsService, AppSettingsService>();
        services.AddSingleton<ITransactionService, TransactionService>();
        services.AddSingleton<ICurrentUserService, CurrentUserService>();
        services.AddSingleton<INotificationService, NotificationService>();

        // Регистрируем ViewModels
        services.AddSingleton<MainWindowViewModel>();
        services.AddTransient<UserLoginViewModel>();
        services.AddTransient<TransactionViewModel>();
        services.AddTransient<TransactionHistoryViewModel>();
        services.AddTransient<UserManagementViewModel>();
        services.AddTransient<UserRegistrationViewModel>();
        services.AddTransient<UserCardViewModel>();
        services.AddTransient<UserMiniCardViewModel>();
        services.AddTransient<CurrencyManagementViewModel>();
        services.AddSingleton<NotificationViewModel>();

        // Регистрируем DbContext как Singleton
        services.AddSingleton<AppDbContext>();

        return services;
    }
} 