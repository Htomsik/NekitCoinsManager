using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using NekitCoinsManager.Shared.DTO;
using NekitCoinsManager.Shared.HttpClient;

namespace NekitCoinsManager.HttpClients
{
    /// <summary>
    /// API-клиент для работы с транзакциями
    /// </summary>
    public class TransactionServiceApiClient : BaseApiClient, ITransactionServiceClient
    {
        /// <summary>
        /// Создает экземпляр API-клиента транзакций
        /// </summary>
        /// <param name="httpClient">HTTP-клиент</param>
        public TransactionServiceApiClient(HttpClient httpClient) : base(httpClient)
        {
        }

        /// <summary>
        /// Получает все транзакции
        /// </summary>
        /// <returns>Коллекция транзакций</returns>
        public async Task<IEnumerable<TransactionDto>> GetTransactionsAsync()
        {
            var result = await GetAsync<List<TransactionDto>>(ApiRoutes.Transaction.Base);
            
            if (!result.success || result.data == null)
                return Enumerable.Empty<TransactionDto>();
                
            return result.data;
        }

        /// <summary>
        /// Получает транзакцию по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор транзакции</param>
        /// <returns>Транзакция или null, если не найдена</returns>
        public async Task<TransactionDto?> GetTransactionByIdAsync(int id)
        {
            var result = await GetAsync<TransactionDto>(ApiRoutes.Transaction.GetById(id));
            
            if (!result.success || result.data == null)
                return null;
                
            return result.data;
        }
    }
} 