using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Core.Services;

/// <summary>
/// Менеджер финансовых операций
/// </summary>
public class MoneyOperationsManager : IMoneyOperationsManager
{
    private readonly MoneyTransferOperationService _transferOperationServiceService;
    private readonly MoneyDepositOperationService _depositOperationService;
    private readonly MoneyConversionOperationService _conversionOperationService;
    private readonly MoneyWelcomeBonusOperationService _welcomeBonusOperationService;

    public MoneyOperationsManager(
        MoneyTransferOperationService transferOperationServiceService,
        MoneyDepositOperationService depositOperationService,
        MoneyConversionOperationService conversionOperationService,
        MoneyWelcomeBonusOperationService welcomeBonusOperationService)
    {
        _transferOperationServiceService = transferOperationServiceService;
        _depositOperationService = depositOperationService;
        _conversionOperationService = conversionOperationService;
        _welcomeBonusOperationService = welcomeBonusOperationService;
    }

    /// <summary>
    /// Выполняет перевод денег между пользователями
    /// </summary>
    public async Task<MoneyOperationResult> TransferAsync(TransferDto transferData)
    {
        return await _transferOperationServiceService.ExecuteAsync(transferData);
    }

    /// <summary>
    /// Пополняет баланс пользователя (депозит)
    /// </summary>
    public async Task<MoneyOperationResult> DepositAsync(DepositDto depositData)
    {
        return await _depositOperationService.ExecuteAsync(depositData);
    }

    /// <summary>
    /// Конвертирует средства пользователя из одной валюты в другую
    /// </summary>
    public async Task<MoneyOperationResult> ConvertAsync(ConversionDto conversionData)
    {
        return await _conversionOperationService.ExecuteAsync(conversionData);
    }

    /// <summary>
    /// Выдает приветственный бонус новому пользователю
    /// </summary>
    public async Task<MoneyOperationResult> GrantWelcomeBonusAsync(WelcomeBonusDto welcomeBonusData)
    {
        return await _welcomeBonusOperationService.ExecuteAsync(welcomeBonusData);
    }
} 