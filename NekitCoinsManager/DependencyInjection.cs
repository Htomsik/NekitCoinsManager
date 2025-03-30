using Microsoft.Extensions.DependencyInjection;
using NekitCoinsManager.Core.Data;
using NekitCoinsManager.Core.Services;
using NekitCoinsManager.Core.Services.AppSettingsService;
using NekitCoinsManager.Services;
using NekitCoinsManager.ViewModels;

namespace NekitCoinsManager;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        // Регистрируем инфраструктурные сервисы
        services.AddSingleton<AppDbContext>();
        services.AddSingleton<INotificationService, NotificationService>();
        services.AddSingleton<IAppSettingsService, AppSettingsService>();
        services.AddSingleton<ICurrentUserService, CurrentUserService>();
        
        // Регистрируем бизнес-сервисы
        services.AddTransient<IAuthService, AuthService>();
        services.AddTransient<IAuthTokenService, AuthTokenService>();
        services.AddTransient<IHardwareInfoService, HardwareInfoService>();
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IPasswordHasherService, PasswordHasherService>();
        services.AddTransient<ICurrencyService, CurrencyService>();
        services.AddTransient<IUserBalanceService, UserBalanceService>();
        services.AddTransient<IUserSettingsService, UserFileSettingsService>();
        services.AddSingleton<ITransactionService, TransactionService>();

        // Регистрируем сервис навигации
        services.AddSingleton<INavigationService, NavigationService>();
        
        // Регистрируем общие компоненты интерфейса
        services.AddSingleton<NotificationViewModel>();
        services.AddTransient<UserMiniCardViewModel>();
        
        // Регистрируем основную вьюмодель окна
        services.AddSingleton<MainWindowViewModel>();

        // Регистрируем вьюмодели экранов
        services.AddTransient<UserLoginViewModel>();
        services.AddTransient<UserRegistrationViewModel>();
        services.AddTransient<UserManagementViewModel>();
        services.AddTransient<UserCardViewModel>();
        services.AddTransient<TransactionViewModel>();
        services.AddTransient<TransactionHistoryViewModel>();
        services.AddTransient<CurrencyManagementViewModel>();
        services.AddTransient<UserTokensViewModel>();

        return services;
    }
} 