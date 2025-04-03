using System.Linq.Expressions;
using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Core.Repositories;

public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    
    // Базовые методы валидации для всех репозиториев
    Task<bool> ExistsByIdAsync(int id);
    Task<(bool isValid, ErrorCode? error)> ValidateEntityAsync(T entity);
    Task<(bool isValid, ErrorCode? error)> ValidateCreateAsync(T entity);
    Task<(bool isValid, ErrorCode? error)> ValidateUpdateAsync(T entity);
    Task<(bool canDelete, ErrorCode? error)> ValidateDeleteAsync(int id);
} 