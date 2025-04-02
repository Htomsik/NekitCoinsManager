using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NekitCoinsManager.Core.Data;
using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Core.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbContext;

    public UserRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _dbContext.Users
            .Include(u => u.Balances)
            .ToListAsync();
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _dbContext.Users
            .Include(u => u.Balances)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User> AddAsync(User entity)
    {
        await _dbContext.Users.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(User entity)
    {
        _dbContext.Entry(entity).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(User entity)
    {
        _dbContext.Users.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<User>> FindAsync(Expression<Func<User, bool>> predicate)
    {
        return await _dbContext.Users
            .Include(u => u.Balances)
            .Where(predicate)
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(Expression<Func<User, bool>> predicate)
    {
        return await _dbContext.Users.AnyAsync(predicate);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _dbContext.Users
            .Include(u => u.Balances)
            .FirstOrDefaultAsync(u => u.Username.Equals(username));
    }

    public async Task<IEnumerable<User>> GetActiveUsersAsync()
    {
        return await _dbContext.Users
            .Include(u => u.Balances)
            .Where(u => !u.IsBankAccount)
            .ToListAsync();
    }

    public async Task<bool> IsUsernameUniqueAsync(string username, int? excludeId = null)
    {
        return !await _dbContext.Users
            .AnyAsync(u => u.Username.Equals(username) && (!excludeId.HasValue || u.Id != excludeId.Value));
    }

    public async Task<bool> IsBankAccountAsync(int userId)
    {
        var user = await GetByIdAsync(userId);
        return user?.IsBankAccount ?? false;
    }

    public async Task<User?> GetBankAccountAsync()
    {
        return await _dbContext.Users
            .Include(u => u.Balances)
            .FirstOrDefaultAsync(u => u.IsBankAccount);
    }
} 