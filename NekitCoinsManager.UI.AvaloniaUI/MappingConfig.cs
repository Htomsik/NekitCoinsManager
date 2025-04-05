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
        TypeAdapterConfig<(int userId, int fromCurrencyId, int toCurrencyId, decimal amount), ConversionDto>
            .NewConfig()
            .Map(dest => dest.UserId, src => src.userId)
            .Map(dest => dest.CurrencyId, src => src.fromCurrencyId)
            .Map(dest => dest.TargetCurrencyId, src => src.toCurrencyId)
            .Map(dest => dest.Amount, src => src.amount);

        // Настройка обратного маппинга Transaction -> TransactionFormModel
        TypeAdapterConfig<Transaction, TransactionFormModel>
            .NewConfig();
            
        // Настройка маппинга Transaction -> TransactionDisplayModel с рекурсивной обработкой дочерних транзакций
        TypeAdapterConfig<Transaction, TransactionDisplayModel>
            .NewConfig()
            .MaxDepth(3) // Ограничиваем глубину рекурсии
            .PreserveReference(true); // Сохраняем ссылки для предотвращения зацикливания
    }
} 