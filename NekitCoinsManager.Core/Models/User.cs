using System;
using System.Collections.Generic;

namespace NekitCoinsManager.Core.Models;

public class User
{
    public int Id { get; set; }
    
    public string Username { get; set; } = string.Empty;
    
    public string PasswordHash { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Признак того, что аккаунт является системным банковским аккаунтом
    /// </summary>
    public bool IsBankAccount { get; set; }
    
    public virtual ICollection<Transaction> SentTransactions { get; set; } = new List<Transaction>();
    
    public virtual ICollection<Transaction> ReceivedTransactions { get; set; } = new List<Transaction>();
    
    public virtual ICollection<UserBalance> Balances { get; set; } = new List<UserBalance>();
    
    public virtual ICollection<UserAuthToken> AuthTokens { get; set; } = new List<UserAuthToken>();
} 