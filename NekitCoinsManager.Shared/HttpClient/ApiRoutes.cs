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
        
    }
}
