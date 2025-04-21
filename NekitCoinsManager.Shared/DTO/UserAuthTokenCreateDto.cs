using System.ComponentModel.DataAnnotations;

namespace NekitCoinsManager.Shared.DTO;

/// <summary>
/// DTO для создания токена авторизации пользователя
/// </summary>
public class UserAuthTokenCreateDto
{
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    [Required]
    public int UserId { get; set; }
    
    /// <summary>
    /// Идентификатор оборудования
    /// </summary>
    [Required]
    public string HardwareId { get; set; } = string.Empty;
} 