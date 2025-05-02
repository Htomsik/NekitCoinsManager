using System;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

namespace NekitCoinsManager.HttpClients
{
    /// <summary>
    /// Расширения для регистрации HTTP-клиентов в DI-контейнере
    /// </summary>
    public static class HttpClientExtensions
    {
        /// <summary>
        /// Регистрирует все HTTP-клиенты API в DI-контейнере
        /// </summary>
        /// <param name="services">Коллекция сервисов</param>
        /// <param name="baseApiUrl">Базовый URL API</param>
        /// <returns>Коллекция сервисов</returns>
        public static IServiceCollection AddApiHttpClients(this IServiceCollection services, string baseApiUrl)
        {
            // Регистрируем AuthHeaderHandler как транзиентный сервис
            services.AddTransient<AuthHeaderHandler>();
            
            // Базовая конфигурация для всех API-клиентов
            services.AddHttpClient("ApiClient", client =>
            {
                client.BaseAddress = new Uri(baseApiUrl);
            }).AddHttpMessageHandler<AuthHeaderHandler>();
            
            return services;
        }
        
        /// <summary>
        /// Регистрирует сервис как синглтон с созданием HttpClient
        /// </summary>
        /// <typeparam name="TInterface">Интерфейс сервиса</typeparam>
        /// <typeparam name="TImplementation">Реализация сервиса</typeparam>
        /// <param name="services">Коллекция сервисов</param>
        /// <param name="factory">Функция для создания экземпляра реализации</param>
        /// <param name="httpClientName">Имя HttpClient</param>
        /// <returns>Коллекция сервисов</returns>
        public static IServiceCollection AddHttpClientAsSingleton<TInterface, TImplementation>(
            this IServiceCollection services, 
            Func<HttpClient, TImplementation> factory,
            string httpClientName = "ApiClient")
            where TInterface : class
            where TImplementation : class, TInterface
        {
            // Получаем фабрику HttpClient и создаем клиент
            services.AddSingleton<TInterface>(sp => {
                var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient(httpClientName);
                return factory(httpClient);
            });
            
            return services;
        }
    }
}
