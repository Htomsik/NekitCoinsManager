using System;
using Microsoft.Extensions.DependencyInjection;

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
    }
}
