using System;

namespace NekitCoinsManager.Shared.DTO
{
    /// <summary>
    /// DTO ответа на запрос аутентификации
    /// </summary>
    public class UserAuthResponseDto
    {
        /// <summary>
        /// Пользователь
        /// </summary>
        public UserDto User { get; set; }
        
        /// <summary>
        /// Токен аутентификации
        /// </summary>
        public UserAuthTokenDto Token { get; set; }
    }
}
