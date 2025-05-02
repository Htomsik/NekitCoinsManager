using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Core.Services;

/// <summary>
/// Менеджер финансовых операций
/// </summary>
public class MoneyOperationsManager : IMoneyOperationsManager
{
    private readonly IMoneyOperationService<TransferOperation> _transferOperationService;
    private readonly IMoneyOperationService<DepositOperation> _depositOperationService;
    private readonly IMoneyOperationService<ConversionOperation> _conversionOperationService;
    private readonly MoneyWelcomeBonusOperationService _welcomeBonusOperationService;

    public MoneyOperationsManager(
        IMoneyOperationService<TransferOperation> transferOperationService,
        IMoneyOperationService<DepositOperation> depositOperationService,
        IMoneyOperationService<ConversionOperation> conversionOperationService,
        MoneyWelcomeBonusOperationService welcomeBonusOperationService)
    {
        _transferOperationService = transferOperationService;
        _depositOperationService = depositOperationService;
        _conversionOperationService = conversionOperationService;
        _welcomeBonusOperationService = welcomeBonusOperationService;
    }

    /// <summary>
    /// Выполняет перевод денег между пользователями
    /// </summary>
    public async Task<MoneyOperationResult> TransferAsync(TransferOperation transferOperationData)
    {
        return await _transferOperationService.ExecuteAsync(transferOperationData);
    }

    /// <summary>
    /// Пополняет баланс пользователя (депозит)
    /// </summary>
    public async Task<MoneyOperationResult> DepositAsync(DepositOperation depositOperationData)
    {
        return await _depositOperationService.ExecuteAsync(depositOperationData);
    }

    /// <summary>
    /// Конвертирует средства пользователя из одной валюты в другую
    /// </summary>
    public async Task<MoneyOperationResult> ConvertAsync(ConversionOperation conversionOperationData)
    {
        return await _conversionOperationService.ExecuteAsync(conversionOperationData);
    }

    /// <summary>
    /// Выдает приветственный бонус новому пользователю
    /// </summary>
    public async Task<MoneyOperationResult> GrantWelcomeBonusAsync(WelcomeBonusOperation welcomeBonusOperationData)
    {
        return await _welcomeBonusOperationService.ExecuteAsync(welcomeBonusOperationData);
    }
} 