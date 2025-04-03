using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NekitCoinsManager.Core.Data;
using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Core.Repositories;

/// <summary>
/// Базовый класс репозитория с общей реализацией методов
/// </summary>
public abstract class BaseRepository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext DbContext;
    protected readonly DbSet<T> DbSet;

    protected BaseRepository(AppDbContext dbContext)
    {
        DbContext = dbContext;
        DbSet = dbContext.Set<T>();
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await DbSet.ToListAsync();
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await DbSet.FindAsync(id);
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        await DbSet.AddAsync(entity);
        await DbContext.SaveChangesAsync();
        return entity;
    }

    public virtual async Task UpdateAsync(T entity)
    {
        DbContext.Entry(entity).State = EntityState.Modified;
        await DbContext.SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(T entity)
    {
        DbSet.Remove(entity);
        await DbContext.SaveChangesAsync();
    }

    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await DbSet.Where(predicate).ToListAsync();
    }

    public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
    {
        return await DbSet.AnyAsync(predicate);
    }

    public virtual async Task<bool> ExistsByIdAsync(int id)
    {
        // Предполагается, что сущность имеет свойство Id
        // Для сущностей с другим именем первичного ключа нужно переопределить этот метод
        var entity = await GetByIdAsync(id);
        return entity != null;
    }

    public virtual async Task<(bool isValid, ErrorCode? error)> ValidateEntityAsync(T entity)
    {
        // Базовая валидация, подклассы должны переопределить этот метод
        // для специфической валидации конкретного типа сущностей
        return (true, null);
    }

    public virtual async Task<(bool isValid, ErrorCode? error)> ValidateCreateAsync(T entity)
    {
        // Базовая валидация при создании, по умолчанию использует общую валидацию
        return await ValidateEntityAsync(entity);
    }

    public virtual async Task<(bool isValid, ErrorCode? error)> ValidateUpdateAsync(T entity)
    {
        // Базовая валидация при обновлении, по умолчанию использует общую валидацию
        return await ValidateEntityAsync(entity);
    }

    public virtual async Task<(bool canDelete, ErrorCode? error)> ValidateDeleteAsync(int id)
    {
        // Базовая проверка перед удалением
        var entity = await GetByIdAsync(id);
        if (entity == null)
            return (false, ErrorCode.CommonEntityNotFound);

        return (true, null);
    }
} 