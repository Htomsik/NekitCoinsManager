using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using NekitCoinsManager.Shared.DTO;
using NekitCoinsManager.Shared.HttpClient;

namespace NekitCoinsManager.HttpClients
{
    /// <summary>
    /// API-клиент для работы с пользователями
    /// </summary>
    public class UserServiceApiClient : BaseApiClient, IUserServiceClient
    {
        /// <summary>
        /// Создает экземпляр API-клиента пользователей
        /// </summary>
        /// <param name="httpClient">HTTP-клиент</param>
        public UserServiceApiClient(HttpClient httpClient) : base(httpClient)
        {
        }

        /// <summary>
        /// Получает список всех пользователей
        /// </summary>
        /// <returns>Список пользователей</returns>
        public async Task<IEnumerable<UserDto>> GetUsersAsync()
        {
            var result = await GetAsync<List<UserDto>>(ApiRoutes.User.Base);
            
            // Если запрос неуспешен, возвращаем пустой список
            if (!result.success || result.data == null)
                return new List<UserDto>();
                
            return result.data;
        }
        
        /// <summary>
        /// Получает пользователя по имени пользователя
        /// </summary>
        /// <param name="username">Имя пользователя</param>
        /// <returns>Пользователь или null, если не найден</returns>
        public async Task<UserDto?> GetUserByUsernameAsync(string username)
        {
            var url = ApiRoutes.User.GetByUsername(username);
            var result = await GetAsync<UserDto>(url);
            
            if (!result.success)
                return null;
                
            return result.data;
        }
        
        /// <summary>
        /// Получает пользователя по идентификатору
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>Пользователь или null, если не найден</returns>
        public async Task<UserDto?> GetUserByIdAsync(int userId)
        {
            var url = ApiRoutes.User.GetById(userId);
            var result = await GetAsync<UserDto>(url);
            
            if (!result.success)
                return null;
                
            return result.data;
        }
        
        /// <summary>
        /// Удаляет пользователя
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>Результат операции</returns>
        public async Task<(bool success, string? error)> DeleteUserAsync(int userId)
        {
            var url = ApiRoutes.User.Delete(userId);
            return await DeleteAsync(url);
        }
    }
} 