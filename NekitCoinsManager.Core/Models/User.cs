using System;
using System.Collections.Generic;

namespace NekitCoinsManager.Core.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public DateTime CreatedAt { get; set; }
    public virtual ICollection<Transaction> SentTransactions { get; set; } = new List<Transaction>();
    public virtual ICollection<Transaction> ReceivedTransactions { get; set; } = new List<Transaction>();
} 