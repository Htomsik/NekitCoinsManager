using System.Collections.Generic;
using System.Threading.Tasks;
using MapsterMapper;
using NekitCoinsManager.Core.Models;
using NekitCoinsManager.Core.Services;
using NekitCoinsManager.Shared.DTO;
using NekitCoinsManager.Shared.HttpClient;

namespace NekitCoinsManager.HttpClients;

/// <summary>
/// Локальный клиент для работы с транзакциями, реализует адаптер между ITransactionService и ITransactionServiceClient
/// </summary>
public class TransactionServiceLocalClient : ITransactionServiceClient
{
    private readonly ITransactionService _transactionService;
    private readonly IMapper _mapper;

    /// <summary>
    /// Инициализирует новый экземпляр класса TransactionServiceLocalClient
    /// </summary>
    /// <param name="transactionService">Оригинальный сервис транзакций</param>
    /// <param name="mapper">Инструмент для маппинга моделей</param>
    public TransactionServiceLocalClient(ITransactionService transactionService, IMapper mapper)
    {
        _transactionService = transactionService;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TransactionDto>> GetTransactionsAsync()
    {
        var transactions = await _transactionService.GetTransactionsAsync();
        return _mapper.Map<IEnumerable<TransactionDto>>(transactions);
    }

    /// <inheritdoc />
    public async Task<TransactionDto?> GetTransactionByIdAsync(int id)
    {
        var transaction = await _transactionService.GetTransactionByIdAsync(id);
        return transaction != null ? _mapper.Map<TransactionDto>(transaction) : null;
    }
} 