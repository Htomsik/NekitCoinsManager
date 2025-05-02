using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using NekitCoinsManager.Core.Models;
using NekitCoinsManager.Core.Services;
using NekitCoinsManager.Shared.DTO;

namespace NekitCoinsManager.API.Controllers;

/// <summary>
/// Контроллер для работы с транзакциями
/// </summary>
public class TransactionController : BaseApiController
{
    private readonly ITransactionService _transactionService;
    
    /// <summary>
    /// Конструктор контроллера
    /// </summary>
    public TransactionController(
        ITransactionService transactionService,
        IMapper mapper) : base(mapper)
    {
        _transactionService = transactionService;
    }
    
    /// <summary>
    /// Получает все транзакции
    /// </summary>
    /// <returns>Список транзакций</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TransactionDto>>> GetTransactions()
    {
        var transactions = await _transactionService.GetTransactionsAsync();
        return Ok(Mapper.Map<List<TransactionDto>>(transactions));
    }
    
    /// <summary>
    /// Получает транзакцию по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор транзакции</param>
    /// <returns>Транзакция</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<TransactionDto>> GetTransaction(int id)
    {
        var transaction = await _transactionService.GetTransactionByIdAsync(id);
        if (transaction == null)
            return NotFound(new { error = "Транзакция не найдена" });
            
        return Ok(Mapper.Map<TransactionDto>(transaction));
    }
} 