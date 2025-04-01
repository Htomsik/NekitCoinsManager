using Mapster;
using NekitCoinsManager.Core.Models;
using System.Collections.Generic;
using System.Linq;

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
            
        // Настройка маппинга Transaction -> TransactionDisplayModel с рекурсивной обработкой дочерних транзакций
        TypeAdapterConfig<Transaction, TransactionDisplayModel>
            .NewConfig()
            .MaxDepth(3) // Ограничиваем глубину рекурсии
            .PreserveReference(true); // Сохраняем ссылки для предотвращения зацикливания
    }
} 