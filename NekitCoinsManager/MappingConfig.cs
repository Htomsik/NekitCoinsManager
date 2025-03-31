using Mapster;
using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Models;

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
            .Map(dest => dest.CreatedAt, _ => System.DateTime.UtcNow)
            .Map(dest => dest.Type, _ => TransactionType.Transfer);

        // Настройка обратного маппинга Transaction -> TransactionFormModel
        TypeAdapterConfig<Transaction, TransactionFormModel>
            .NewConfig();
    }
} 