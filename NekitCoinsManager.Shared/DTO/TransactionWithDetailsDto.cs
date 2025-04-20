using System;
using System.Collections.Generic;

namespace NekitCoinsManager.Shared.DTO;

/// <summary>
/// Расширенная DTO для транзакции с включением вложенных данных
/// </summary>
public class TransactionWithDetailsDto : TransactionDto
{
    /// <summary>
    /// Родительская транзакция (если текущая является дочерней)
    /// </summary>
    public TransactionDto? ParentTransaction { get; set; }
    
    /// <summary>
    /// Связанные дочерние транзакции
    /// </summary>
    public List<TransactionDto> ChildTransactions { get; set; } = new();
    
    /// <summary>
    /// Признак наличия дочерних транзакций
    /// </summary>
    public bool HasChildTransactions => ChildTransactions.Count > 0;
} 