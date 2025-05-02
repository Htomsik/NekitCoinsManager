using System.ComponentModel.DataAnnotations;

namespace NekitCoinsManager.Shared.DTO;

/// <summary>
/// DTO для валидации токена авторизации пользователя
/// </summary>
public class UserAuthTokenValidateDto
{
    /// <summary>
    /// Значение токена авторизации
    /// </summary>
    [Required]
    public string Token { get; set; } = string.Empty;
    
    /// <summary>
    /// Идентификатор оборудования
    /// </summary>
    [Required]
    public string HardwareId { get; set; } = string.Empty;
} 