using Microsoft.Extensions.DependencyInjection;
using NekitCoinsManager.Core.Data;
using NekitCoinsManager.Core.Repositories;
using NekitCoinsManager.Core.Services;

namespace NekitCoinsManager.Core;

/// <summary>
/// Класс для конфигурации сервисов Core
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Добавляет сервисы ядра приложения в контейнер зависимостей
    /// </summary>
    /// <param name="services">Коллекция сервисов</param>
    /// <returns>Измененная коллекция сервисов</returns>
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        // Регистрируем DbContext - используем Scoped для лучшей производительности и отслеживания изменений
        services.AddDbContext<AppDbContext>(ServiceLifetime.Scoped);
        
        // Регистрация репозиториев
        services.AddScoped<ICurrencyRepository, CurrencyRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserAuthTokenRepository, UserAuthTokenRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IUserBalanceRepository, UserBalanceRepository>();
        
        // Регистрируем базовые сервисы без зависимостей от других сервисов
        services.AddTransient<IPasswordHasherService, PasswordHasherService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICurrencyService, CurrencyService>();
        services.AddScoped<IUserBalanceService, UserBalanceService>();
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<ICurrencyConversionService, CurrencyConversionService>();
        services.AddScoped<IDbTransactionService, DbTransactionService>();
        
        // Регистрируем сервисы для операций с деньгами
        services.AddScoped<MoneyTransferOperationService>();
        services.AddScoped<MoneyDepositOperationService>();
        services.AddScoped<MoneyConversionOperationService>();
        services.AddScoped<MoneyWelcomeBonusOperationService>();
        services.AddScoped<IMoneyOperationsManager, MoneyOperationsManager>();
        
        // Сервисы с зависимостями от других сервисов регистрируем в последнюю очередь
        services.AddScoped<IAuthTokenService, AuthTokenService>();

        return services;
    }
} 