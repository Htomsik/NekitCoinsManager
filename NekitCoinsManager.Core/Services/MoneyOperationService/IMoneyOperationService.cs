using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Core.Services;

/// <summary>
/// Базовый интерфейс для всех денежных операций
/// </summary>
/// <typeparam name="TDto">Тип DTO для операции</typeparam>
public interface IMoneyOperationService<in TDto> where TDto : MoneyOperation
{
    /// <summary>
    /// Выполняет денежную операцию
    /// </summary>
    /// <param name="operationData">Данные для выполнения операции</param>
    /// <returns>Результат выполнения операции</returns>
    Task<MoneyOperationResult> ExecuteAsync(TDto operationData);
    
    /// <summary>
    /// Валидирует данные операции
    /// </summary>
    /// <param name="operationData">Данные для валидации</param>
    /// <returns>Результат валидации: успех и сообщение об ошибке</returns>
    Task<(bool isValid, string? errorMessage)> ValidateAsync(TDto operationData);
} 