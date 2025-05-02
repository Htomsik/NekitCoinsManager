using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using NekitCoinsManager.Shared.DTO;
using NekitCoinsManager.Shared.HttpClient;

namespace NekitCoinsManager.HttpClients
{
    /// <summary>
    /// API-клиент для работы с балансами пользователей
    /// </summary>
    public class UserBalanceServiceApiClient : BaseApiClient, IUserBalanceServiceClient
    {
        /// <summary>
        /// Создает экземпляр API-клиента балансов пользователей
        /// </summary>
        /// <param name="httpClient">HTTP-клиент</param>
        public UserBalanceServiceApiClient(HttpClient httpClient) : base(httpClient)
        {
        }

        /// <summary>
        /// Получает все балансы пользователя
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>Список балансов пользователя</returns>
        public async Task<IEnumerable<UserBalanceDto>> GetUserBalancesAsync(int userId)
        {
            var url = ApiRoutes.UserBalance.GetUserBalances(userId);
            var result = await GetAsync<List<UserBalanceDto>>(url);
            
            // Если запрос неуспешен, возвращаем пустой список
            if (!result.success || result.data == null)
                return new List<UserBalanceDto>();
                
            return result.data;
        }
        
        /// <summary>
        /// Получает баланс пользователя по указанной валюте
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="currencyId">Идентификатор валюты</param>
        /// <returns>Баланс пользователя или null, если не найден</returns>
        public async Task<UserBalanceDto?> GetUserBalanceAsync(int userId, int currencyId)
        {
            var url = ApiRoutes.UserBalance.GetUserBalance(userId, currencyId);
            var result = await GetAsync<UserBalanceDto>(url);
            
            if (!result.success)
                return null;
                
            return result.data;
        }
    }
} 