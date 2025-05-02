using System;

namespace NekitCoinsManager.Shared.HttpClient
{
    /// <summary>
    /// Константы путей к API-эндпоинтам
    /// </summary>
    public static class ApiRoutes
    {
        /// <summary>
        /// Базовый путь к API аутентификации пользователей
        /// </summary>
        public static class UserAuth
        {
            /// <summary>
            /// Базовый путь к API аутентификации
            /// </summary>
            public const string Base = "api/UserAuth";
            
            /// <summary>
            /// Эндпоинт для проверки пароля
            /// </summary>
            public const string VerifyPassword = Base + "/verifyPassword";
            
            /// <summary>
            /// Эндпоинт для аутентификации пользователя
            /// </summary>
            public const string Authenticate = Base + "/authenticate";
            
            /// <summary>
            /// Эндпоинт для регистрации пользователя
            /// </summary>
            public const string Register = Base + "/register";
            
            /// <summary>
            /// Эндпоинт для восстановления сессии
            /// </summary>
            public const string RestoreSession = Base + "/restoreSession";
            
            /// <summary>
            /// Эндпоинт для выхода из системы
            /// </summary>
            public const string Logout = Base + "/logout";
        }
        
        /// <summary>
        /// Базовый путь к API токенов авторизации
        /// </summary>
        public static class AuthToken
        {
            /// <summary>
            /// Базовый путь к API токенов
            /// </summary>
            public const string Base = "api/AuthToken";
            
            /// <summary>
            /// Эндпоинт для создания токена
            /// </summary>
            public const string Create = Base + "/create";
            
            /// <summary>
            /// Эндпоинт для валидации токена
            /// </summary>
            public const string Validate = Base + "/validate";
            
            /// <summary>
            /// Эндпоинт для деактивации токена
            /// </summary>
            public const string Deactivate = Base + "/deactivate";
            
            /// <summary>
            /// Эндпоинт для деактивации всех токенов пользователя
            /// </summary>
            public const string DeactivateAll = Base + "/deactivateAll";
            
            /// <summary>
            /// Эндпоинт для получения всех токенов пользователя
            /// </summary>
            public const string GetUserTokens = Base + "/getUserTokens";
        }
        
        /// <summary>
        /// Базовый путь к API пользователей
        /// </summary>
        public static class User
        {
            /// <summary>
            /// Базовый путь к API пользователей
            /// </summary>
            public const string Base = "api/User";
            
            /// <summary>
            /// Эндпоинт для получения пользователя по идентификатору
            /// </summary>
            /// <param name="id">Идентификатор пользователя</param>
            public static string GetById(int id) => $"{Base}/{id}";
            
            /// <summary>
            /// Эндпоинт для получения пользователя по имени пользователя
            /// </summary>
            /// <param name="username">Имя пользователя</param>
            public static string GetByUsername(string username) => $"{Base}/byUsername/{username}";
            
            /// <summary>
            /// Эндпоинт для удаления пользователя
            /// </summary>
            /// <param name="id">Идентификатор пользователя</param>
            public static string Delete(int id) => $"{Base}/{id}";
        }
    }
}
