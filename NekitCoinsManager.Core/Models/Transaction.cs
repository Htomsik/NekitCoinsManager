using System;

namespace NekitCoinsManager.Core.Models;

public class Transaction
{
    public int Id { get; set; }
    public int FromUserId { get; set; }
    public int ToUserId { get; set; }
    public int CurrencyId { get; set; }
    public decimal Amount { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    
    public virtual User FromUser { get; set; } = null!;
    public virtual User ToUser { get; set; } = null!;
    public virtual Currency Currency { get; set; } = null!;
} 