using System;
using System.Threading.Tasks;

namespace NekitCoinsManager.Core.Services
{
    /// <summary>
    /// Сервис для управления транзакциями базы данных
    /// </summary>
    public interface IDbTransactionService
    {
        /// <summary>
        /// Выполняет действие внутри транзакции базы данных
        /// </summary>
        /// <typeparam name="T">Тип возвращаемого значения</typeparam>
        /// <param name="action">Асинхронное действие, которое необходимо выполнить внутри транзакции. 
        /// Должно возвращать (bool success, T result), где success - признак успешности операции</param>
        /// <returns>Результат выполнения действия</returns>
        Task<(bool success, T result)> ExecuteInTransactionAsync<T>(Func<Task<(bool success, T result)>> action);
        
        /// <summary>
        /// Выполняет действие внутри транзакции базы данных с возвращаемым значением
        /// </summary>
        /// <typeparam name="T">Тип дополнительного возвращаемого значения</typeparam>
        /// <param name="action">Асинхронное действие, которое необходимо выполнить внутри транзакции.
        /// Должно возвращать bool - признак успешности операции</param>
        /// <returns>Признак успешности выполнения действия</returns>
        Task<bool> ExecuteInTransactionAsync(Func<Task<bool>> action);
    }
} 