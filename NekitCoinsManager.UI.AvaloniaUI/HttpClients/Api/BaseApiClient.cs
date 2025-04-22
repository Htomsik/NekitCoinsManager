using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace NekitCoinsManager.HttpClients.Api
{
    /// <summary>
    /// Базовый класс для HTTP клиентов API
    /// </summary>
    public abstract class BaseApiClient
    {
        protected readonly HttpClient _httpClient;
        protected readonly JsonSerializerOptions _jsonOptions;

        /// <summary>
        /// Создает экземпляр базового API клиента
        /// </summary>
        /// <param name="httpClient">HTTP клиент для запросов к API</param>
        protected BaseApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = false
            };
        }

        /// <summary>
        /// Выполняет GET запрос к API
        /// </summary>
        /// <typeparam name="TResponse">Тип ожидаемого ответа</typeparam>
        /// <param name="url">URL эндпоинта</param>
        /// <returns>Результат запроса</returns>
        protected async Task<TResponse?> GetAsync<TResponse>(string url)
        {
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<TResponse>(_jsonOptions);
        }

        /// <summary>
        /// Выполняет POST запрос к API
        /// </summary>
        /// <typeparam name="TRequest">Тип отправляемых данных</typeparam>
        /// <typeparam name="TResponse">Тип ожидаемого ответа</typeparam>
        /// <param name="url">URL эндпоинта</param>
        /// <param name="data">Данные для отправки</param>
        /// <returns>Результат запроса</returns>
        protected async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest data)
        {
            var response = await _httpClient.PostAsJsonAsync(url, data, _jsonOptions);
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<TResponse>(_jsonOptions);
        }

        /// <summary>
        /// Выполняет POST запрос к API без ожидания ответа
        /// </summary>
        /// <typeparam name="TRequest">Тип отправляемых данных</typeparam>
        /// <param name="url">URL эндпоинта</param>
        /// <param name="data">Данные для отправки</param>
        /// <returns>True, если запрос выполнен успешно</returns>
        protected async Task<bool> PostAsync<TRequest>(string url, TRequest data)
        {
            var response = await _httpClient.PostAsJsonAsync(url, data, _jsonOptions);
            return response.IsSuccessStatusCode;
        }
    }
} 