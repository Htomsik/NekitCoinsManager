using System;

namespace NekitCoinsManager.Core.Models;

public class UserBalance
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CurrencyId { get; set; }
    public decimal Amount { get; set; }
    public DateTime LastUpdateTime { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual Currency Currency { get; set; } = null!;
} 