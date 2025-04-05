using System;
using System.Collections.Generic;

namespace NekitCoinsManager.Shared.DTO;

/// <summary>
/// DTO модель пользователя
/// </summary>
public class UserDto
{
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Имя пользователя
    /// </summary>
    public string Username { get; set; } = string.Empty;
    
    /// <summary>
    /// Дата создания аккаунта
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Признак того, что аккаунт является системным банковским аккаунтом
    /// </summary>
    public bool IsBankAccount { get; set; }
} 