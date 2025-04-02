using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NekitCoinsManager.Core.Data;
using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Core.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly AppDbContext _dbContext;

    public TransactionRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Transaction>> GetAllAsync()
    {
        return await _dbContext.Transactions
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<Transaction?> GetByIdAsync(int id)
    {
        return await _dbContext.Transactions
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<Transaction> AddAsync(Transaction entity)
    {
        await _dbContext.Transactions.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(Transaction entity)
    {
        _dbContext.Entry(entity).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Transaction entity)
    {
        _dbContext.Transactions.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<Transaction>> FindAsync(Expression<Func<Transaction, bool>> predicate)
    {
        return await _dbContext.Transactions
            .Where(predicate)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(Expression<Func<Transaction, bool>> predicate)
    {
        return await _dbContext.Transactions.AnyAsync(predicate);
    }

    public async Task<IEnumerable<Transaction>> GetUserTransactionsAsync(int userId)
    {
        return await _dbContext.Transactions
            .Where(t => t.FromUserId == userId || t.ToUserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetUserSentTransactionsAsync(int userId)
    {
        return await _dbContext.Transactions
            .Where(t => t.FromUserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetUserReceivedTransactionsAsync(int userId)
    {
        return await _dbContext.Transactions
            .Where(t => t.ToUserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetRelatedTransactionsAsync(int transactionId)
    {
        var transaction = await GetByIdAsync(transactionId);
        if (transaction == null)
            return Enumerable.Empty<Transaction>();

        var relatedTransactions = new List<Transaction>();
        
        // Загружаем связанные транзакции только при необходимости
        if (transaction.ParentTransactionId.HasValue)
        {
            var parentTransaction = await _dbContext.Transactions
                .FirstOrDefaultAsync(t => t.Id == transaction.ParentTransactionId);
            if (parentTransaction != null)
            {
                relatedTransactions.Add(parentTransaction);
            }
        }

        var childTransactions = await _dbContext.Transactions
            .Where(t => t.ParentTransactionId == transactionId)
            .ToListAsync();
        relatedTransactions.AddRange(childTransactions);

        return relatedTransactions;
    }

    public async Task<bool> HasTransactionsWithCurrencyAsync(int currencyId)
    {
        return await _dbContext.Transactions
            .AnyAsync(t => t.CurrencyId == currencyId);
    }

    public async Task<IEnumerable<Transaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbContext.Transactions
            .Where(t => t.CreatedAt >= startDate && t.CreatedAt <= endDate)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<decimal> GetUserBalanceInCurrencyAsync(int userId, int currencyId)
    {
        var received = await _dbContext.Transactions
            .Where(t => t.ToUserId == userId && t.CurrencyId == currencyId)
            .SumAsync(t => t.Amount);

        var sent = await _dbContext.Transactions
            .Where(t => t.FromUserId == userId && t.CurrencyId == currencyId)
            .SumAsync(t => t.Amount);

        return received - sent;
    }
} 