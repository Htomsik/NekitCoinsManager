using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using NekitCoinsManager.Shared.DTO;
using NekitCoinsManager.Shared.HttpClient;

namespace NekitCoinsManager.HttpClients
{
    /// <summary>
    /// API-клиент для работы с токенами авторизации
    /// </summary>
    public class AuthTokenServiceApiClient : BaseApiClient, IAuthTokenServiceClient
    {
        /// <summary>
        /// Создает экземпляр API-клиента токенов авторизации
        /// </summary>
        /// <param name="httpClient">HTTP-клиент</param>
        public AuthTokenServiceApiClient(HttpClient httpClient) : base(httpClient)
        {
        }

        /// <summary>
        /// Создает новый токен авторизации для пользователя
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <param name="hardwareId">ID оборудования</param>
        /// <returns>Созданный токен</returns>
        public async Task<UserAuthTokenDto> CreateTokenAsync(int userId, string hardwareId)
        {
            var request = new UserAuthTokenCreateDto
            {
                UserId = userId,
                HardwareId = hardwareId
            };
            
            var result = await PostAsync<UserAuthTokenCreateDto, UserAuthTokenDto>(ApiRoutes.AuthToken.Create, request);
            
            if (!result.success || result.data == null)
                return new UserAuthTokenDto();
                
            return result.data;
        }

        /// <summary>
        /// Проверяет валидность токена
        /// </summary>
        /// <param name="token">Значение токена</param>
        /// <param name="hardwareId">ID оборудования</param>
        /// <returns>Токен, если он валидный, иначе null</returns>
        public async Task<UserAuthTokenDto?> ValidateTokenAsync(string token, string hardwareId)
        {
            var request = new UserAuthTokenValidateDto
            {
                Token = token,
                HardwareId = hardwareId
            };
            
            var result = await PostAsync<UserAuthTokenValidateDto, UserAuthTokenDto>(ApiRoutes.AuthToken.Validate, request);
            
            if (!result.success)
                return null;
                
            return result.data;
        }

        /// <summary>
        /// Деактивирует указанный токен
        /// </summary>
        /// <param name="tokenId">ID токена</param>
        public async Task DeactivateTokenAsync(int tokenId)
        {
            var url = $"{ApiRoutes.AuthToken.Deactivate}/{tokenId}";
            
            await PostAsync<object>(url, null);
        }

        /// <summary>
        /// Деактивирует все токены пользователя
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        public async Task DeactivateAllUserTokensAsync(int userId)
        {
            var url = $"{ApiRoutes.AuthToken.DeactivateAll}/{userId}";
            
            await PostAsync<object>(url, null);
        }

        /// <summary>
        /// Получает все токены пользователя
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <returns>Список токенов</returns>
        public async Task<IEnumerable<UserAuthTokenDto>> GetUserTokensAsync(int userId)
        {
            var url = $"{ApiRoutes.AuthToken.Base}/user/{userId}";
            
            var result = await GetAsync<List<UserAuthTokenDto>>(url);
            
            if (!result.success || result.data == null)
                return new List<UserAuthTokenDto>();
                
            return result.data;
        }
    }
} 