using System;

namespace NekitCoinsManager.Core.Models;

public class UserAuthToken
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    
    public string Token { get; set; } = string.Empty;
    
    public string HardwareId { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime ExpiresAt { get; set; }
    
    public bool IsActive { get; set; }
    
    public virtual User User { get; set; } = null!;
} 