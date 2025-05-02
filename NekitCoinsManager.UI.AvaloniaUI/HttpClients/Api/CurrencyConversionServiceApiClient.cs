using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using NekitCoinsManager.Shared.DTO;
using NekitCoinsManager.Shared.DTO.Operations;
using NekitCoinsManager.Shared.HttpClient;

namespace NekitCoinsManager.HttpClients
{
    /// <summary>
    /// API-клиент для работы с конвертацией валют
    /// </summary>
    public class CurrencyConversionServiceApiClient : BaseApiClient, ICurrencyConversionServiceClient
    {
        /// <summary>
        /// Создает экземпляр API-клиента конвертации валют
        /// </summary>
        /// <param name="httpClient">HTTP-клиент</param>
        public CurrencyConversionServiceApiClient(HttpClient httpClient) : base(httpClient)
        {
        }

        /// <summary>
        /// Конвертирует сумму из одной валюты в другую
        /// </summary>
        /// <param name="conversionDto">Данные для конвертации</param>
        /// <returns>Сконвертированная сумма</returns>
        public async Task<decimal> ConvertCurrencyAsync(CurrencyConversionDto conversionDto)
        {
            var result = await PostAsync<CurrencyConversionDto, CurrencyConversionResultDto>(
                ApiRoutes.CurrencyConversion.Convert, 
                conversionDto);
                
            if (!result.success || result.data == null)
                return 0;
                
            return result.data.Amount;
        }

        /// <summary>
        /// Получает текущий обменный курс между двумя валютами
        /// </summary>
        /// <param name="queryDto">Параметры запроса</param>
        /// <returns>Обменный курс</returns>
        public async Task<decimal> GetExchangeRateAsync(CurrencyExchangeRateOperationDto queryDto)
        {
            // Формируем URL с параметрами запроса
            var url = $"{ApiRoutes.CurrencyConversion.Rate}?fromCurrencyCode={queryDto.FromCurrencyCode}&toCurrencyCode={queryDto.ToCurrencyCode}";
            
            var result = await GetAsync<CurrencyExchangeRateResultDto>(url);
            
            if (!result.success || result.data == null)
                return 0;
                
            return result.data.Rate;
        }

        /// <summary>
        /// Получает все доступные обменные курсы
        /// </summary>
        /// <returns>Словарь обменных курсов [fromCurrencyCode][toCurrencyCode] = rate</returns>
        public async Task<Dictionary<string, Dictionary<string, decimal>>> GetAllExchangeRatesAsync()
        {
            var result = await GetAsync<CurrencyExchangeRatesDto>(ApiRoutes.CurrencyConversion.Rates);
            
            if (!result.success || result.data == null)
                return new Dictionary<string, Dictionary<string, decimal>>();
                
            return result.data.Rates;
        }
    }
} 