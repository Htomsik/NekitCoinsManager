using NekitCoinsManager.Shared.DTO;

namespace NekitCoinsManager.Shared.HttpClient;

/// <summary>
/// Клиентский интерфейс для работы с транзакциями
/// </summary>
public interface ITransactionServiceClient
{
    /// <summary>
    /// Получает все транзакции
    /// </summary>
    Task<IEnumerable<TransactionDto>> GetTransactionsAsync();
    
    /// <summary>
    /// Получает транзакцию по идентификатору
    /// </summary>
    Task<TransactionDto?> GetTransactionByIdAsync(int id);
} 