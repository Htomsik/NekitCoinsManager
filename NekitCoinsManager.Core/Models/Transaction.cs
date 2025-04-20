using System;
using System.Collections.Generic;

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
    public TransactionType Type { get; set; } = TransactionType.Transfer;
    
    /// <summary>
    /// ID родительской транзакции (если текущая транзакция связана с другой).
    /// Например, для комиссии это ID основной транзакции, для транзакции зачисления при конвертации - ID транзакции списания.
    /// </summary>
    public int? ParentTransactionId { get; set; }
    
    public virtual User FromUser { get; set; } = null!;
    public virtual User ToUser { get; set; } = null!;
    public virtual Currency Currency { get; set; } = null!;
    
    /// <summary>
    /// Родительская транзакция, если текущая транзакция связана с другой
    /// </summary>
    public virtual Transaction? ParentTransaction { get; set; }
    
    /// <summary>
    /// Дочерние транзакции, связанные с текущей.
    /// Например, для транзакции списания при конвертации - транзакция зачисления целевой валюты,
    /// для основной транзакции - транзакции комиссий.
    /// </summary>
    public virtual ICollection<Transaction> ChildTransactions { get; set; } = new List<Transaction>();
} 