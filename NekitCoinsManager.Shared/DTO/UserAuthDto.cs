using System.ComponentModel.DataAnnotations;

namespace NekitCoinsManager.Shared.DTO;

/// <summary>
/// DTO для аутентификации пользователя (вход в систему)
/// </summary>
public class UserLoginDto
{
    /// <summary>
    /// Имя пользователя
    /// </summary>
    [Required]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Имя пользователя должно содержать от 3 до 50 символов")]
    public string Username { get; set; } = string.Empty;
    
    /// <summary>
    /// Пароль
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль должен содержать не менее 6 символов")]
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// DTO для регистрации нового пользователя (наследуется от DTO входа)
/// </summary>
public class UserRegistrationDto : UserLoginDto
{
    /// <summary>
    /// Подтверждение пароля
    /// </summary>
    [Required]
    [Compare("Password", ErrorMessage = "Пароли не совпадают")]
    public string ConfirmPassword { get; set; } = string.Empty;
} 