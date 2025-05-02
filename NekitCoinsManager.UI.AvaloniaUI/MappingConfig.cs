using System;
using Mapster;
using NekitCoinsManager.Models;
using NekitCoinsManager.Shared.DTO;
using NekitCoinsManager.Shared.DTO.Operations;

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
        // Настройка маппинга TransactionFormModel -> TransferDto
        TypeAdapterConfig<TransactionFormModel, TransferDto>
            .NewConfig()
            .Map(dest => dest.UserId, src => src.FromUserId)
            .Map(dest => dest.RecipientId, src => src.ToUserId);
            
        // Настройка маппинга TransactionFormModel -> DepositDto
        TypeAdapterConfig<TransactionFormModel, DepositDto>
            .NewConfig()
            .Map(dest => dest.UserId, src => src.ToUserId);

        // Настройка маппинга TransactionConversionDisplayModel -> ConversionDto
        TypeAdapterConfig<TransactionConversionDisplayModel, ConversionDto>
            .NewConfig()
            .Map(dest => dest.CurrencyId, src => src.FromCurrency.Id)
            .Map(dest => dest.TargetCurrencyId, src => src.ToCurrency.Id);

        // Настройка маппинга TransactionDto -> TransactionDisplayModel
        TypeAdapterConfig<TransactionDto, TransactionDisplayModel>
            .NewConfig()
            .MaxDepth(3)
            .PreserveReference(true);
    }
} 