using System.Net.Http;
using System.Threading.Tasks;
using NekitCoinsManager.Shared.DTO;
using NekitCoinsManager.Shared.DTO.Operations;
using NekitCoinsManager.Shared.HttpClient;

namespace NekitCoinsManager.HttpClients
{
    /// <summary>
    /// API-клиент для работы с денежными операциями
    /// </summary>
    public class MoneyOperationsServiceApiClient : BaseApiClient, IMoneyOperationsServiceClient
    {
        /// <summary>
        /// Создает экземпляр API-клиента денежных операций
        /// </summary>
        /// <param name="httpClient">HTTP-клиент</param>
        public MoneyOperationsServiceApiClient(HttpClient httpClient) : base(httpClient)
        {
        }

        /// <summary>
        /// Выполняет перевод денег между пользователями
        /// </summary>
        /// <param name="transferDto">Данные для перевода</param>
        /// <returns>Результат операции перевода</returns>
        public async Task<MoneyOperationResultDto> TransferAsync(TransferDto transferDto)
        {
            var result = await PostAsync<TransferDto, MoneyOperationResultDto>(
                ApiRoutes.MoneyOperations.Transfer, transferDto);
            
            if (!result.success || result.data == null)
                return new MoneyOperationResultDto { Success = false, Error = result.error ?? "Ошибка при выполнении перевода" };
                
            return result.data;
        }
        
        /// <summary>
        /// Пополняет баланс пользователя (депозит)
        /// </summary>
        /// <param name="depositDto">Данные для пополнения</param>
        /// <returns>Результат операции пополнения</returns>
        public async Task<MoneyOperationResultDto> DepositAsync(DepositDto depositDto)
        {
            var result = await PostAsync<DepositDto, MoneyOperationResultDto>(
                ApiRoutes.MoneyOperations.Deposit, depositDto);
            
            if (!result.success || result.data == null)
                return new MoneyOperationResultDto { Success = false, Error = result.error ?? "Ошибка при выполнении пополнения" };
                
            return result.data;
        }
        
        /// <summary>
        /// Конвертирует средства пользователя из одной валюты в другую
        /// </summary>
        /// <param name="conversionDto">Данные для конвертации</param>
        /// <returns>Результат операции конвертации</returns>
        public async Task<MoneyOperationResultDto> ConvertAsync(ConversionDto conversionDto)
        {
            var result = await PostAsync<ConversionDto, MoneyOperationResultDto>(
                ApiRoutes.MoneyOperations.Convert, conversionDto);
            
            if (!result.success || result.data == null)
                return new MoneyOperationResultDto { Success = false, Error = result.error ?? "Ошибка при выполнении конвертации" };
                
            return result.data;
        }
    }
} 