using System;
using Mapster;
using NekitCoinsManager.Core.Models;
using NekitCoinsManager.Models;
using NekitCoinsManager.Shared.DTO;

namespace NekitCoinsManager;

/// <summary>
/// Конфигурация маппинга для Mapster
/// </summary>
public static class MappingConfig
{
    /// <summary>
    /// Инициализирует конфигурацию маппинга
    /// </summary>
    public static void ConfigureMapping()
    {
        // Настройка маппинга TransactionFormModel -> Transaction
        TypeAdapterConfig<TransactionFormModel, Transaction>
            .NewConfig()
            .Map(dest => dest.CreatedAt, _ => DateTime.UtcNow)
            .Map(dest => dest.Type, _ => TransactionType.Transfer);

        // Настройка маппинга TransactionFormModel -> TransferDto
        TypeAdapterConfig<TransactionFormModel, TransferDto>
            .NewConfig()
            .Map(dest => dest.UserId, src => src.FromUserId)
            .Map(dest => dest.RecipientId, src => src.ToUserId);
            
        // Настройка маппинга TransactionFormModel -> DepositDto
        TypeAdapterConfig<TransactionFormModel, DepositDto>
            .NewConfig()
            .Map(dest => dest.UserId, src => src.ToUserId);
            
        // Настройка маппинга для ConversionDto
        TypeAdapterConfig<(decimal amount, Currency fromCurrency, Currency toCurrency, User fromUser, User toUser), ConversionDto>
            .NewConfig()
            .Map(dest => dest.Amount, src => src.amount)
            .Map(dest => dest.CurrencyId, src => src.fromCurrency.Id)
            .Map(dest => dest.TargetCurrencyId, src => src.toCurrency.Id)
            .Map(dest => dest.UserId, src => src.fromUser.Id);
        
        // Настройка маппинга TransactionConversionDisplayModel -> ConversionDto
        TypeAdapterConfig<TransactionConversionDisplayModel, ConversionDto>
            .NewConfig()
            .Map(dest => dest.Amount, src => src.Amount)
            .Map(dest => dest.CurrencyId, src => src.FromCurrency.Id)
            .Map(dest => dest.TargetCurrencyId, src => src.ToCurrency.Id)
            .Map(dest => dest.UserId, src => src.UserId);

        // Настройка обратного маппинга Transaction -> TransactionFormModel
        TypeAdapterConfig<Transaction, TransactionFormModel>
            .NewConfig();
            
        // Настройка маппинга Transaction -> TransactionDisplayModel с рекурсивной обработкой дочерних транзакций
        TypeAdapterConfig<Transaction, TransactionDisplayModel>
            .NewConfig()
            .MaxDepth(3) 
            .PreserveReference(true); // Для рекурсивного разрешения связанных транзакций

        // Настройка маппинга TransactionType -> TransactionTypeDto
        TypeAdapterConfig<TransactionType, TransactionTypeDto>
            .NewConfig()
            .MapToConstructor(true);
            
        TypeAdapterConfig<TransactionTypeDto, TransactionType>
            .NewConfig()
            .MapToConstructor(true);
    }
} 