using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using NekitCoinsManager.HttpClients;
using NekitCoinsManager.Models;
using NekitCoinsManager.Services;
using NekitCoinsManager.Shared.HttpClient;
using NekitCoinsManager.ViewModels;

namespace NekitCoinsManager;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services, IAppSettingsService appSettingsService)
    {
        // Регистрируем сервисы
        services.AddTransient<IMapper, Mapper>();
        services.AddSingleton<INotificationService, NotificationService>();
        services.AddSingleton(appSettingsService);
        services.AddSingleton<ICurrentUserService, CurrentUserService>();
        services.AddTransient<IUserSettingsService, UserFileSettingsService>();
        services.AddTransient<IHardwareInfoService, HardwareInfoService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddSingleton<INavigationService, NavigationService>();
        
        // Регистрируем HttpClient и сервисы для его управления
        services.AddApiHttpClients();
        
        // Регистрируем API клиенты
        services.AddHttpClient<IAuthTokenServiceClient, AuthTokenServiceApiClient>(SettingsConstants.HttpClientName);
        services.AddHttpClient<IUserAuthServiceClient, UserAuthServiceApiClient>(SettingsConstants.HttpClientName);
        services.AddHttpClient<IUserServiceClient, UserServiceApiClient>(SettingsConstants.HttpClientName);
        services.AddHttpClient<IUserBalanceServiceClient, UserBalanceServiceApiClient>(SettingsConstants.HttpClientName);
        services.AddHttpClient<ICurrencyServiceClient, CurrencyServiceApiClient>(SettingsConstants.HttpClientName);
        services.AddHttpClient<ICurrencyConversionServiceClient, CurrencyConversionServiceApiClient>(SettingsConstants.HttpClientName);
        services.AddHttpClient<ITransactionServiceClient, TransactionServiceApiClient>(SettingsConstants.HttpClientName);
        services.AddHttpClientAsSingleton<IMoneyOperationsServiceClient, MoneyOperationsServiceApiClient>(
            httpClient => new MoneyOperationsServiceApiClient(httpClient), SettingsConstants.HttpClientName);
        
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