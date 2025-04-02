using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NekitCoinsManager.Core.Data;
using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Core.Repositories;

public class UserBalanceRepository : IUserBalanceRepository
{
    private readonly AppDbContext _dbContext;

    public UserBalanceRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<UserBalance>> GetAllAsync()
    {
        return await _dbContext.UserBalances
            .Include(ub => ub.Currency)
            .ToListAsync();
    }

    public async Task<UserBalance?> GetByIdAsync(int id)
    {
        return await _dbContext.UserBalances
            .Include(ub => ub.Currency)
            .FirstOrDefaultAsync(ub => ub.Id == id);
    }

    public async Task<UserBalance> AddAsync(UserBalance entity)
    {
        await _dbContext.UserBalances.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(UserBalance entity)
    {
        _dbContext.Entry(entity).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(UserBalance entity)
    {
        _dbContext.UserBalances.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<UserBalance>> FindAsync(Expression<Func<UserBalance, bool>> predicate)
    {
        return await _dbContext.UserBalances
            .Include(ub => ub.Currency)
            .Where(predicate)
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(Expression<Func<UserBalance, bool>> predicate)
    {
        return await _dbContext.UserBalances.AnyAsync(predicate);
    }

    public async Task<IEnumerable<UserBalance>> GetUserBalancesAsync(int userId)
    {
        return await _dbContext.UserBalances
            .Include(ub => ub.Currency)
            .Where(ub => ub.UserId == userId)
            .ToListAsync();
    }

    public async Task<UserBalance?> GetUserBalanceAsync(int userId, int currencyId)
    {
        return await _dbContext.UserBalances
            .Include(ub => ub.Currency)
            .FirstOrDefaultAsync(ub => ub.UserId == userId && ub.CurrencyId == currencyId);
    }

    public async Task<bool> HasBalanceAsync(int userId, int currencyId)
    {
        return await _dbContext.UserBalances
            .AnyAsync(ub => ub.UserId == userId && ub.CurrencyId == currencyId);
    }

    public async Task<bool> HasEnoughBalanceAsync(int userId, int currencyId, decimal amount)
    {
        var balance = await GetUserBalanceAsync(userId, currencyId);
        return balance != null && balance.Amount >= amount;
    }

    public async Task<IEnumerable<UserBalance>> GetBalancesByCurrencyAsync(int currencyId)
    {
        return await _dbContext.UserBalances
            .Include(ub => ub.Currency)
            .Where(ub => ub.CurrencyId == currencyId)
            .ToListAsync();
    }

    public async Task<bool> HasBalancesWithCurrencyAsync(int currencyId)
    {
        return await _dbContext.UserBalances
            .AnyAsync(b => b.CurrencyId == currencyId);
    }
} 