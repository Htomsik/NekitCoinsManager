using System.ComponentModel.DataAnnotations;

namespace NekitCoinsManager.Shared.DTO;

public class UserAuthLogoutDto
{
    /// <summary>
    /// Имя пользователя
    /// </summary>
    [Required]
    public int UserId { get; set; } 
    

    /// <summary>
    /// Идентификатор устройства для генерации токена
    /// </summary>
    [Required]
    public string HardwareId { get; set; } = string.Empty;
}