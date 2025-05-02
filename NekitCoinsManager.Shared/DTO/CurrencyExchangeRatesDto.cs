using System.Collections.Generic;

namespace NekitCoinsManager.Shared.DTO;

/// <summary>
/// DTO с таблицей всех обменных курсов валют
/// </summary>
public class CurrencyExchangeRatesDto
{
    /// <summary>
    /// Таблица обменных курсов: [исходная валюта][целевая валюта] = курс
    /// </summary>
    public Dictionary<string, Dictionary<string, decimal>> Rates { get; set; } = new();
} 