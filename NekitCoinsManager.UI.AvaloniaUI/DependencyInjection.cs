using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using NekitCoinsManager.Core;
using NekitCoinsManager.HttpClients;
using NekitCoinsManager.Services;
using NekitCoinsManager.Shared.HttpClient;
using NekitCoinsManager.ViewModels;
using System;

namespace NekitCoinsManager;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        // Добавляем сервисы ядра (В будующем уедут на API)
        services.AddCoreServices();
        
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
        
        // Регистрируем временные локальные клиенты
        services.AddScoped<IUserBalanceServiceClient, UserBalanceServiceLocalClient>();
        services.AddScoped<IUserServiceClient, UserServiceLocalClient>();
        services.AddScoped<ICurrencyConversionServiceClient, CurrencyConversionServiceLocalClient>();
        services.AddScoped<ICurrencyServiceClient, CurrencyServiceLocalClient>();
        services.AddScoped<IMoneyOperationsServiceClient, MoneyOperationsServiceLocalClient>();
        services.AddScoped<ITransactionServiceClient, TransactionServiceLocalClient>();
        
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
        services.AddTransient<UserMiniCardViewModel>();
        
        // Регистрируем конкретные реализации TransactionViewModel
        services.AddTransient<TransactionMainTransferViewModel>();
        services.AddTransient<TransactionMainDepositViewModel>();
        services.AddTransient<TransactionMainConversionViewModel>();
        
        services.AddTransient<TransactionHistoryViewModel>();
        services.AddTransient<CurrencyManagementViewModel>();
        services.AddTransient<UserTokensViewModel>();

        return services;
    }
} 