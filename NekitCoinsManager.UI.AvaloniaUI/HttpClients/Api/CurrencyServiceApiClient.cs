using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using NekitCoinsManager.Shared.DTO;
using NekitCoinsManager.Shared.HttpClient;

namespace NekitCoinsManager.HttpClients
{
    /// <summary>
    /// API-клиент для работы с валютами
    /// </summary>
    public class CurrencyServiceApiClient : BaseApiClient, ICurrencyServiceClient
    {
        /// <summary>
        /// Создает экземпляр API-клиента валют
        /// </summary>
        /// <param name="httpClient">HTTP-клиент</param>
        public CurrencyServiceApiClient(HttpClient httpClient) : base(httpClient)
        {
        }

        /// <summary>
        /// Получает список всех валют
        /// </summary>
        /// <returns>Список валют</returns>
        public async Task<IEnumerable<CurrencyDto>> GetCurrenciesAsync()
        {
            var result = await GetAsync<List<CurrencyDto>>(ApiRoutes.Currency.Base);
            
            // Если запрос неуспешен, возвращаем пустой список
            if (!result.success || result.data == null)
                return new List<CurrencyDto>();
                
            return result.data;
        }
        
        /// <summary>
        /// Получает валюту по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор валюты</param>
        /// <returns>Валюта или null, если не найдена</returns>
        public async Task<CurrencyDto?> GetCurrencyByIdAsync(int id)
        {
            var url = ApiRoutes.Currency.GetById(id);
            var result = await GetAsync<CurrencyDto>(url);
            
            if (!result.success || result.data == null)
                return null;
                
            return result.data;
        }
        
        /// <summary>
        /// Получает валюту по коду
        /// </summary>
        /// <param name="code">Код валюты</param>
        /// <returns>Валюта или null, если не найдена</returns>
        public async Task<CurrencyDto?> GetCurrencyByCodeAsync(string code)
        {
            var url = ApiRoutes.Currency.GetByCode(code);
            var result = await GetAsync<CurrencyDto>(url);
            
            if (!result.success || result.data == null)
                return null;
                
            return result.data;
        }
        
        /// <summary>
        /// Добавляет новую валюту
        /// </summary>
        /// <param name="currency">Данные валюты</param>
        /// <returns>Результат операции</returns>
        public async Task<(bool success, string? error)> AddCurrencyAsync(CurrencyDto currency)
        {
            var result = await PostAsync<CurrencyDto, object>(ApiRoutes.Currency.Base, currency);
            return (result.success, result.error);
        }
        
        /// <summary>
        /// Обновляет данные валюты
        /// </summary>
        /// <param name="currency">Обновленные данные валюты</param>
        /// <returns>Результат операции</returns>
        public async Task<(bool success, string? error)> UpdateCurrencyAsync(CurrencyDto currency)
        {
            var result = await PutAsync<CurrencyDto, object>(ApiRoutes.Currency.Base, currency);
            return (result.success, result.error);
        }
        
        /// <summary>
        /// Удаляет валюту
        /// </summary>
        /// <param name="id">Идентификатор валюты</param>
        /// <returns>Результат операции</returns>
        public async Task<(bool success, string? error)> DeleteCurrencyAsync(int id)
        {
            var url = ApiRoutes.Currency.Delete(id);
            var result = await DeleteAsync<object>(url);
            return (result.success, result.error);
        }
        
        /// <summary>
        /// Обновляет обменный курс валюты
        /// </summary>
        /// <param name="currencyId">Идентификатор валюты</param>
        /// <param name="newRate">Новый обменный курс</param>
        /// <returns>Результат операции</returns>
        public async Task<(bool success, string? error)> UpdateExchangeRateAsync(int currencyId, decimal newRate)
        {
            var rateInfo = new CurrencyExchangeRateDto
            {
                Id = currencyId,
                ExchangeRate = newRate
            };
            
            var result = await PatchAsync<CurrencyExchangeRateDto, object>(
                ApiRoutes.Currency.UpdateRate, rateInfo);
                
            return (result.success, result.error);
        }
    }
} 