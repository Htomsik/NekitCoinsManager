using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using NekitCoinsManager.HttpClients;
using NekitCoinsManager.Services;
using NekitCoinsManager.Shared.HttpClient;
using NekitCoinsManager.ViewModels;

namespace NekitCoinsManager;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        // Регистрируем сервисы
        services.AddTransient<IMapper, Mapper>();
        services.AddSingleton<INotificationService, NotificationService>();
        services.AddSingleton<IAppSettingsService, AppSettingsService>();
        services.AddSingleton<ICurrentUserService, CurrentUserService>();
        services.AddTransient<IUserSettingsService, UserFileSettingsService>();
        services.AddTransient<IHardwareInfoService, HardwareInfoService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddSingleton<INavigationService, NavigationService>();
        
        // Регистрируем HttpClient для API
        services.AddApiHttpClients("http://localhost:5122/");
        
        // Регистрируем API клиенты
        services.AddHttpClient<IAuthTokenServiceClient, AuthTokenServiceApiClient>("ApiClient");
        services.AddHttpClient<IUserAuthServiceClient, UserAuthServiceApiClient>("ApiClient");
        services.AddHttpClient<IUserServiceClient, UserServiceApiClient>("ApiClient");
        services.AddHttpClient<IUserBalanceServiceClient, UserBalanceServiceApiClient>("ApiClient");
        services.AddHttpClient<ICurrencyServiceClient, CurrencyServiceApiClient>("ApiClient");
        services.AddHttpClient<ICurrencyConversionServiceClient, CurrencyConversionServiceApiClient>("ApiClient");
        services.AddHttpClient<ITransactionServiceClient, TransactionServiceApiClient>("ApiClient");
        services.AddHttpClientAsSingleton<IMoneyOperationsServiceClient, MoneyOperationsServiceApiClient>(
            httpClient => new MoneyOperationsServiceApiClient(httpClient));
        
        // Регистрируем вьюмодели
        services.AddSingleton<MainWindowViewModel>();
        services.AddTransient<UserLoginViewModel>();
        services.AddTransient<UserRegistrationViewModel>();
        services.AddTransient<UserManagementViewModel>();
        services.AddTransient<UserCardViewModel>();
        services.AddTransient<TransactionTransferViewModel>();
        services.AddTransient<TransactionDepositViewModel>();
        services.AddTransient<TransactionConversionViewModel>();
        services.AddSingleton<NotificationViewModel>();
        services.AddSingleton<UserMiniCardViewModel>();
        
        // Регистрируем конкретные реализации TransactionViewModel
        services.AddTransient<TransactionMainTransferViewModel>();
        services.AddTransient<TransactionMainDepositViewModel>();
        services.AddTransient<TransactionMainConversionViewModel>();
        
        // Регистрируем TransactionHistoryViewModel как transient
        services.AddTransient<TransactionHistoryViewModel>();
        services.AddTransient<CurrencyManagementViewModel>();
        services.AddTransient<UserTokensViewModel>();

        return services;
    }
} 