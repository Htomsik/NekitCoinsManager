using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using NekitCoinsManager.Services;
using NekitCoinsManager.Shared.Constants;

namespace NekitCoinsManager.HttpClients
{
    /// <summary>
    /// Обработчик HTTP-запросов для автоматического добавления заголовков аутентификации
    /// </summary>
    public class AuthHeaderHandler : DelegatingHandler
    {
        private readonly ICurrentUserService _currentUserService;
        
        /// <summary>
        /// Создает экземпляр обработчика заголовков аутентификации
        /// </summary>
        /// <param name="currentUserService">Сервис текущего пользователя</param>
        public AuthHeaderHandler(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }
        
        /// <summary>
        /// Добавляет заголовки аутентификации к исходящим запросам
        /// </summary>
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Добавляем HardwareId к каждому запросу
            var hardwareId = _currentUserService.Settings.HardwareId;
            if (!string.IsNullOrEmpty(hardwareId))
            {
                request.Headers.Add(HttpHeaderNames.HardwareId, hardwareId);
            }
            
            // Добавляем токен авторизации, если он есть
            var authToken = _currentUserService.Settings.AuthToken;
            if (!string.IsNullOrEmpty(authToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue(
                    HttpHeaderNames.BearerPrefix.Trim(), authToken);
            }
            
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
