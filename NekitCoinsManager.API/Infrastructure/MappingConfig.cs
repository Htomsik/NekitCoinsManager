using Mapster;
using NekitCoinsManager.Core.Models;
using NekitCoinsManager.Shared.DTO;
using NekitCoinsManager.Shared.DTO.Operations;

namespace NekitCoinsManager.API.Infrastructure;

/// <summary>
/// Конфигурация маппинга для Mapster в API проекте
/// </summary>
public static class MappingConfig
{
    /// <summary>
    /// Инициализирует конфигурацию маппинга
    /// </summary>
    public static void ConfigureMapping()
    {
        // Маппинг Core модели -> DTO
        ConfigureCoreToDtoMappings();
        
        // Маппинг DTO -> Core модели
        ConfigureDtoToCoreMappings();
        
        // Маппинг для операций
        ConfigureOperationsMappings();
    }
    
    /// <summary>
    /// Настройка маппинга Core моделей в DTO
    /// </summary>
    private static void ConfigureCoreToDtoMappings()
    {
        // User -> UserDto
        TypeAdapterConfig<User, UserDto>
            .NewConfig()
            .Map(dest => dest.Balances, src => src.Balances);

        // UserBalance -> UserBalanceDto
        TypeAdapterConfig<UserBalance, UserBalanceDto>.NewConfig();
        
        // Currency -> CurrencyDto
        TypeAdapterConfig<Currency, CurrencyDto>.NewConfig();
        
        // UserAuthToken -> UserAuthTokenDto
        TypeAdapterConfig<UserAuthToken, UserAuthTokenDto>
            .NewConfig()
            .Map(dest => dest.Token, src => src.Token)
            .Map(dest => dest.UserId, src => src.UserId);
        
        // Transaction -> TransactionDto
        TypeAdapterConfig<Transaction, TransactionDto>
            .NewConfig();
        
        // Transaction -> TransactionWithDetailsDto
        TypeAdapterConfig<Transaction, TransactionWithDetailsDto>
            .NewConfig()
            .MaxDepth(3)
            .PreserveReference(true);
        
        // TransactionType -> TransactionTypeDto
        TypeAdapterConfig<TransactionType, TransactionTypeDto>
            .NewConfig()
            .MapToConstructor(true);
    }

    /// <summary>
    /// Настройка маппинга DTO в Core модели
    /// </summary>
    private static void ConfigureDtoToCoreMappings()
    {
        // UserDto -> User
        TypeAdapterConfig<UserDto, User>
            .NewConfig()
            .Ignore(dest => dest.PasswordHash)
            .Ignore(dest => dest.SentTransactions)
            .Ignore(dest => dest.ReceivedTransactions)
            .Ignore(dest => dest.AuthTokens);
            
        // UserBalanceDto -> UserBalance
        TypeAdapterConfig<UserBalanceDto, UserBalance>.NewConfig();
        
        // CurrencyDto -> Currency
        TypeAdapterConfig<CurrencyDto, Currency>.NewConfig();
        
        // UserAuthTokenDto -> UserAuthToken 
        TypeAdapterConfig<UserAuthTokenDto, UserAuthToken>.NewConfig();
        
        // TransactionDto -> Transaction
        TypeAdapterConfig<TransactionDto, Transaction>
            .NewConfig()
            .Ignore(dest => dest.FromUser)
            .Ignore(dest => dest.ToUser);
        
        // TransactionTypeDto -> TransactionType
        TypeAdapterConfig<TransactionTypeDto, TransactionType>
            .NewConfig()
            .MapToConstructor(true);
    }

    /// <summary>
    /// Настройка маппинга для операций
    /// </summary>
    private static void ConfigureOperationsMappings()
    {
        // MoneyOperationResult -> MoneyOperationResultDto
        TypeAdapterConfig<MoneyOperationResult, MoneyOperationResultDto>.NewConfig();
        
        // Маппинг для операций трансфера
        TypeAdapterConfig<TransferOperation, TransferDto>
            .NewConfig();
        
        TypeAdapterConfig<TransferDto, TransferOperation>
            .NewConfig();
        
        // Маппинг для операций пополнения
        TypeAdapterConfig<DepositOperation, DepositDto>
            .NewConfig();
        
        TypeAdapterConfig<DepositDto, DepositOperation>
            .NewConfig();
        
        // Маппинг для операций конвертации
        TypeAdapterConfig<ConversionOperation, ConversionDto>
            .NewConfig();
        
        TypeAdapterConfig<ConversionDto, ConversionOperation>
            .NewConfig();
            
        // Маппинг для приветственного бонуса
        TypeAdapterConfig<WelcomeBonusOperation, WelcomeBonusDto>
            .NewConfig()
            .Map(dest => dest.UserId, src => src.NewUserId);
            
        TypeAdapterConfig<WelcomeBonusDto, WelcomeBonusOperation>
            .NewConfig()
            .Map(dest => dest.NewUserId, src => src.UserId);
    }
} 