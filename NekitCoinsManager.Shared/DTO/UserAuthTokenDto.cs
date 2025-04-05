using System;

namespace NekitCoinsManager.Shared.DTO;

/// <summary>
/// DTO для токена авторизации пользователя
/// </summary>
public class UserAuthTokenDto
{
    /// <summary>
    /// Идентификатор токена
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public int UserId { get; set; }
    
    /// <summary>
    /// Значение токена
    /// </summary>
    public string Token { get; set; } = string.Empty;
    
    /// <summary>
    /// Идентификатор оборудования
    /// </summary>
    public string HardwareId { get; set; } = string.Empty;
    
    /// <summary>
    /// Дата создания токена
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Дата истечения срока действия токена
    /// </summary>
    public DateTime ExpiresAt { get; set; }
    
    /// <summary>
    /// Признак активности токена
    /// </summary>
    public bool IsActive { get; set; }
} 