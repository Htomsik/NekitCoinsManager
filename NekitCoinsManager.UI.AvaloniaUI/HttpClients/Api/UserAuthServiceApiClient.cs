using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using NekitCoinsManager.Shared.DTO;
using NekitCoinsManager.Shared.HttpClient;

namespace NekitCoinsManager.HttpClients
{
    /// <summary>
    /// API-клиент для работы с аутентификацией и регистрацией пользователей
    /// </summary>
    public class UserAuthServiceApiClient : BaseApiClient, IUserAuthServiceClient
    {
        /// <summary>
        /// Создает экземпляр API-клиента аутентификации
        /// </summary>
        /// <param name="httpClient">HTTP-клиент</param>
        public UserAuthServiceApiClient(HttpClient httpClient) : base(httpClient)
        {
        }
        
        /// <summary>
        /// Проверяет пароль пользователя
        /// </summary>
        /// <param name="request">Данные для проверки пароля</param>
        /// <returns>Результат операции</returns>
        public async Task<(bool success, string? error)> VerifyPasswordAsync(UserAuthLoginDto request)
        {
            var result = await PostAsync<UserAuthLoginDto, object>(ApiRoutes.UserAuth.VerifyPassword, request);
            return (result.success, result.error);
        }
        
        /// <summary>
        /// Аутентифицирует пользователя и возвращает его данные при успешной аутентификации
        /// </summary>
        /// <param name="request">Данные для аутентификации</param>
        /// <returns>Результат операции, пользователь и токен аутентификации при успехе</returns>
        public async Task<(bool success, string? error, UserDto? user, UserAuthTokenDto? token)> AuthenticateUserAsync(UserAuthLoginDto request)
        {
            var result = await PostAsync<UserAuthLoginDto, UserAuthResponseDto>(ApiRoutes.UserAuth.Authenticate, request);
            
            if (!result.success || result.data == null)
                return (result.success, result.error, null, null);
                
            return (true, null, result.data.User, result.data.Token);
        }
        
        /// <summary>
        /// Регистрирует нового пользователя
        /// </summary>
        /// <param name="request">Данные для регистрации</param>
        /// <returns>Результат операции</returns>
        public async Task<(bool success, string? error)> RegisterUserAsync(UserAuthRegistrationDto request)
        {
            var result = await PostAsync<UserAuthRegistrationDto, object>(ApiRoutes.UserAuth.Register, request);
            return (result.success, result.error);
        }
        
        /// <summary>
        /// Восстанавливает сессию пользователя по токену аутентификации
        /// </summary>
        /// <param name="request">Данные для восстановления сессии</param>
        /// <returns>Результат операции, пользователь при успехе</returns>
        public async Task<(bool success, string? error, UserDto? user)> RestoreSessionAsync(UserAuthTokenValidateDto request)
        {
            var result = await PostAsync<UserAuthTokenValidateDto, UserDto>(ApiRoutes.UserAuth.RestoreSession, request);
            
            if (!result.success || result.data == null)
                return (result.success, result.error, null);
                
            return (true, null, result.data);
        }
        
        /// <summary>
        /// Выполняет выход пользователя из системы, деактивируя токен
        /// </summary>
        /// <param name="request">Данные для выхода из системы</param>
        /// <returns>Результат операции</returns>
        public async Task<(bool success, string? error)> LogoutAsync(UserAuthLogoutDto request)
        {
            var result = await PostAsync<UserAuthLogoutDto, object>(ApiRoutes.UserAuth.Logout, request);
            return (result.success, result.error);
        }
        

    }
}
