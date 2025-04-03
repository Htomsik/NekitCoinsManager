using System;
using System.Linq;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using NekitCoinsManager.Core.Data;
using NekitCoinsManager.Core.Repositories;
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
        
        // Регистрация репозиториев
        services.AddScoped<ICurrencyRepository, CurrencyRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserAuthTokenRepository, UserAuthTokenRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IUserBalanceRepository, UserBalanceRepository>();
        
        // Регистрируем инфраструктурные сервисы
        services.AddTransient<IMapper, Mapper>();
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
        services.AddScoped<ICurrencyConversionService, CurrencyConversionService>();
        services.AddScoped<IDbTransactionService, DbTransactionService>();
        
        // Регистрируем сервисы для операций с деньгами
        services.AddScoped<MoneyTransferOperationService>();
        services.AddScoped<MoneyDepositOperationService>();
        services.AddScoped<MoneyConversionOperationService>();
        services.AddScoped<IMoneyOperationsManager, MoneyOperationsManager>();

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
        services.AddTransient<TransactionTransferViewModel>();
        services.AddTransient<TransactionDepositViewModel>();
        services.AddTransient<TransactionConversionViewModel>();
        
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