using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NekitCoinsManager.Core.Data;
using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Core.Repositories;

public class CurrencyRepository : ICurrencyRepository
{
    private readonly AppDbContext _dbContext;

    public CurrencyRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Models.Currency>> GetAllAsync()
    {
        return await _dbContext.Currencies.ToListAsync();
    }

    public async Task<Models.Currency?> GetByIdAsync(int id)
    {
        return await _dbContext.Currencies.FindAsync(id);
    }

    public async Task<Models.Currency> AddAsync(Models.Currency entity)
    {
        await _dbContext.Currencies.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(Models.Currency entity)
    {
        _dbContext.Entry(entity).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Models.Currency entity)
    {
        _dbContext.Currencies.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<Models.Currency>> FindAsync(Expression<Func<Models.Currency, bool>> predicate)
    {
        return await _dbContext.Currencies.Where(predicate).ToListAsync();
    }

    public async Task<bool> ExistsAsync(Expression<Func<Models.Currency, bool>> predicate)
    {
        return await _dbContext.Currencies.AnyAsync(predicate);
    }

    public async Task<Models.Currency?> GetByCodeAsync(string code)
    {
        return await _dbContext.Currencies
            .FirstOrDefaultAsync(c => c.Code.Equals(code));
    }

    public async Task<IEnumerable<Models.Currency>> GetActiveCurrenciesAsync()
    {
        return await _dbContext.Currencies
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Models.Currency?> GetDefaultCurrencyAsync()
    {
        return await _dbContext.Currencies
            .FirstOrDefaultAsync(c => c.IsDefaultForNewUsers);
    }

    public async Task<bool> IsCodeUniqueAsync(string code, int? excludeId = null)
    {
        return !await _dbContext.Currencies
            .AnyAsync(c => c.Code.Equals(code) && (!excludeId.HasValue || c.Id != excludeId.Value));
    }
} 