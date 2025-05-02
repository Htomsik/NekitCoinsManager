using System.Threading.Tasks;
using MapsterMapper;
using NekitCoinsManager.Core.Models;
using NekitCoinsManager.Core.Services;
using NekitCoinsManager.Shared.DTO;
using NekitCoinsManager.Shared.DTO.Operations;
using NekitCoinsManager.Shared.HttpClient;

namespace NekitCoinsManager.HttpClients;

/// <summary>
/// Локальная реализация клиента денежных операций
/// </summary>
public class MoneyOperationsServiceLocalClient : IMoneyOperationsServiceClient
{
    private readonly IMoneyOperationsManager _moneyOperationsManager;
    private readonly IMapper _mapper;

    /// <summary>
    /// Создает экземпляр локального клиента денежных операций
    /// </summary>
    /// <param name="moneyOperationsManager">Оригинальный менеджер денежных операций</param>
    /// <param name="mapper">Инструмент для маппинга моделей</param>
    public MoneyOperationsServiceLocalClient(IMoneyOperationsManager moneyOperationsManager, IMapper mapper)
    {
        _moneyOperationsManager = moneyOperationsManager;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<MoneyOperationResultDto> TransferAsync(TransferDto transferDto)
    {
        // Маппим DTO на модель операции
        var transferOperation = _mapper.Map<TransferOperation>(transferDto);
        
        // Выполняем операцию перевода
        var result = await _moneyOperationsManager.TransferAsync(transferOperation);

        // Преобразуем результат в DTO через маппер
        return _mapper.Map<MoneyOperationResultDto>(result);
    }

    /// <inheritdoc />
    public async Task<MoneyOperationResultDto> DepositAsync(DepositDto depositDto)
    {
        // Маппим DTO на модель операции
        var depositOperation = _mapper.Map<DepositOperation>(depositDto);
        
        // Выполняем операцию пополнения
        var result = await _moneyOperationsManager.DepositAsync(depositOperation);
        
        // Преобразуем результат в DTO через маппер
        return _mapper.Map<MoneyOperationResultDto>(result);
    }

    /// <inheritdoc />
    public async Task<MoneyOperationResultDto> ConvertAsync(ConversionDto conversionDto)
    {
        // Маппим DTO на модель операции
        var conversionOperation = _mapper.Map<ConversionOperation>(conversionDto);
        
        // Выполняем операцию конвертации
        var result = await _moneyOperationsManager.ConvertAsync(conversionOperation);
        
        // Преобразуем результат в DTO через маппер
        return _mapper.Map<MoneyOperationResultDto>(result);
    }
} 