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
        // Регистрируем DbContext - используем Scoped для лучшей производительности и отслеживания изменений
        services.AddDbContext<AppDbContext>(ServiceLifetime.Scoped);
        
        // Регистрируем инфраструктурные сервисы
        services.AddSingleton<INotificationService, NotificationService>();
        services.AddSingleton<IAppSettingsService, AppSettingsService>();
        services.AddSingleton<ICurrentUserService, CurrentUserService>();
        
        // Регистрируем бизнес-сервисы
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAuthTokenService, AuthTokenService>();
        services.AddTransient<IHardwareInfoService, HardwareInfoService>();
        services.AddScoped<IUserService, UserService>();
        services.AddTransient<IPasswordHasherService, PasswordHasherService>();
        services.AddScoped<ICurrencyService, CurrencyService>();
        services.AddScoped<IUserBalanceService, UserBalanceService>();
        services.AddTransient<IUserSettingsService, UserFileSettingsService>();
        services.AddScoped<ITransactionService, TransactionService>();

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