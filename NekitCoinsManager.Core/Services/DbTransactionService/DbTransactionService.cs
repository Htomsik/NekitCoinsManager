using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NekitCoinsManager.Core.Data;

namespace NekitCoinsManager.Core.Services
{
    /// <summary>
    /// Сервис для управления транзакциями базы данных
    /// </summary>
    public class DbTransactionService : IDbTransactionService
    {
        private readonly AppDbContext _dbContext;

        public DbTransactionService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Выполняет действие внутри транзакции базы данных
        /// </summary>
        /// <typeparam name="T">Тип возвращаемого значения</typeparam>
        /// <param name="action">Асинхронное действие, которое необходимо выполнить внутри транзакции.
        /// Должно возвращать (bool success, T result), где success - признак успешности операции</param>
        /// <returns>Результат выполнения действия</returns>
        public async Task<(bool success, T result)> ExecuteInTransactionAsync<T>(Func<Task<(bool success, T result)>> action)
        {
            // Создаем транзакцию
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            
            try
            {
                // Выполняем действие
                var (success, result) = await action();
                
                // Фиксируем транзакцию, только если операция прошла успешно
                if (success)
                {
                    await transaction.CommitAsync();
                    return (true, result);
                }
                else
                {
                    // Если операция не прошла успешно, откатываем транзакцию
                    await transaction.RollbackAsync();
                    return (false, result);
                }
            }
            catch (Exception ex)
            {
                // При возникновении исключения, откатываем транзакцию
                await transaction.RollbackAsync();
                
                // Здесь можно добавить логирование ошибки
                
                // Возвращаем признак неуспешного выполнения
                // Мы не можем вернуть значение по умолчанию, так как не знаем, что это за тип
                // Поэтому выбрасываем исключение
                throw new InvalidOperationException($"Ошибка при выполнении транзакции: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Выполняет действие внутри транзакции базы данных
        /// </summary>
        /// <param name="action">Асинхронное действие, которое необходимо выполнить внутри транзакции.
        /// Должно возвращать bool - признак успешности операции</param>
        /// <returns>Признак успешности выполнения действия</returns>
        public async Task<bool> ExecuteInTransactionAsync(Func<Task<bool>> action)
        {
            // Создаем транзакцию
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            
            try
            {
                // Выполняем действие
                bool success = await action();
                
                // Фиксируем транзакцию, только если операция прошла успешно
                if (success)
                {
                    await transaction.CommitAsync();
                    return true;
                }
                else
                {
                    // Если операция не прошла успешно, откатываем транзакцию
                    await transaction.RollbackAsync();
                    return false;
                }
            }
            catch (Exception ex)
            {
                // При возникновении исключения, откатываем транзакцию
                await transaction.RollbackAsync();
                
                // Здесь можно добавить логирование ошибки
                
                // Возвращаем признак неуспешного выполнения
                return false;
            }
        }
    }
} 