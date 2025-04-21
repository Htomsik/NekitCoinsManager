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
    private readonly List<ITransactionObserverClient> _observers = new List<ITransactionObserverClient>();

    /// <summary>
    /// Инициализирует новый экземпляр класса TransactionServiceLocalClient
    /// </summary>
    /// <param name="transactionService">Оригинальный сервис транзакций</param>
    /// <param name="mapper">Инструмент для маппинга моделей</param>
    public TransactionServiceLocalClient(ITransactionService transactionService, IMapper mapper)
    {
        _transactionService = transactionService;
        _mapper = mapper;
        
        // Подписываемся на изменения транзакций через адаптер
        _transactionService.Subscribe(new TransactionObserverAdapter(this));
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

    /// <inheritdoc />
    public async Task<(bool success, string? error)> AddTransactionAsync(TransactionDto transaction)
    {
        var transactionModel = _mapper.Map<Transaction>(transaction);
        return await _transactionService.AddTransactionAsync(transactionModel);
    }

    /// <inheritdoc />
    public async Task<(bool isValid, string? errorMessage)> ValidateTransactionAsync(TransactionDto transaction)
    {
        var transactionModel = _mapper.Map<Transaction>(transaction);
        return await _transactionService.ValidateTransactionAsync(transactionModel);
    }

    /// <inheritdoc />
    public void Subscribe(ITransactionObserverClient observer)
    {
        if (!_observers.Contains(observer))
        {
            _observers.Add(observer);
        }
    }

    /// <inheritdoc />
    public void NotifyObservers()
    {
        foreach (var observer in _observers)
        {
            observer.OnTransactionsChanged();
        }
    }

    /// <summary>
    /// Адаптер для преобразования наблюдателя транзакций из Core в наблюдателя клиента
    /// </summary>
    private class TransactionObserverAdapter : ITransactionObserver
    {
        private readonly TransactionServiceLocalClient _client;

        public TransactionObserverAdapter(TransactionServiceLocalClient client)
        {
            _client = client;
        }

        public void OnTransactionsChanged()
        {
            _client.NotifyObservers();
        }
    }
} 