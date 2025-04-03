using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NekitCoinsManager.Core.Data;
using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Core.Repositories;

public class UserBalanceRepository : BaseRepository<UserBalance>, IUserBalanceRepository
{
    public UserBalanceRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public override async Task<IEnumerable<UserBalance>> GetAllAsync()
    {
        return await DbSet
            .Include(ub => ub.Currency)
            .ToListAsync();
    }

    public override async Task<UserBalance?> GetByIdAsync(int id)
    {
        return await DbSet
            .Include(ub => ub.Currency)
            .FirstOrDefaultAsync(ub => ub.Id == id);
    }

    public override async Task<IEnumerable<UserBalance>> FindAsync(Expression<Func<UserBalance, bool>> predicate)
    {
        return await DbSet
            .Include(ub => ub.Currency)
            .Where(predicate)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserBalance>> GetUserBalancesAsync(int userId)
    {
        return await DbSet
            .Include(ub => ub.Currency)
            .Where(ub => ub.UserId == userId)
            .ToListAsync();
    }

    public async Task<UserBalance?> GetUserBalanceAsync(int userId, int currencyId)
    {
        return await DbSet
            .Include(ub => ub.Currency)
            .FirstOrDefaultAsync(ub => ub.UserId == userId && ub.CurrencyId == currencyId);
    }

    public async Task<bool> HasBalanceAsync(int userId, int currencyId)
    {
        return await DbSet
            .AnyAsync(ub => ub.UserId == userId && ub.CurrencyId == currencyId);
    }

    public async Task<bool> HasEnoughBalanceAsync(int userId, int currencyId, decimal amount)
    {
        var balance = await GetUserBalanceAsync(userId, currencyId);
        return balance != null && balance.Amount >= amount;
    }

    public async Task<IEnumerable<UserBalance>> GetBalancesByCurrencyAsync(int currencyId)
    {
        return await DbSet
            .Include(ub => ub.Currency)
            .Where(ub => ub.CurrencyId == currencyId)
            .ToListAsync();
    }

    public async Task<bool> HasBalancesWithCurrencyAsync(int currencyId)
    {
        return await DbSet
            .AnyAsync(b => b.CurrencyId == currencyId);
    }

    // Реализация методов валидации
    public override async Task<(bool isValid, ErrorCode? error)> ValidateEntityAsync(UserBalance entity)
    {
        // Проверка обязательных полей
        if (entity.UserId <= 0)
            return (false, ErrorCode.BalanceUserIdInvalid);

        if (entity.CurrencyId <= 0)
            return (false, ErrorCode.TransactionCurrencyIdInvalid);

        if (entity.Amount < 0)
            return (false, ErrorCode.BalanceAmountNegative);

        return (true, null);
    }

    public override async Task<(bool isValid, ErrorCode? error)> ValidateCreateAsync(UserBalance entity)
    {
        // Базовая валидация сущности
        var baseValidation = await ValidateEntityAsync(entity);
        if (!baseValidation.isValid)
            return baseValidation;

        // Проверка на дубликаты
        var existing = await GetUserBalanceAsync(entity.UserId, entity.CurrencyId);
        if (existing != null)
            return (false, ErrorCode.BalanceAlreadyExists);

        return (true, null);
    }

    public override async Task<(bool isValid, ErrorCode? error)> ValidateUpdateAsync(UserBalance entity)
    {
        // Проверка существования
        var existingBalance = await GetByIdAsync(entity.Id);
        if (existingBalance == null)
            return (false, ErrorCode.BalanceNotFound);

        // Базовая валидация полей
        if (entity.Amount < 0)
            return (false, ErrorCode.BalanceAmountNegative);

        return (true, null);
    }

    public async Task<(bool isValid, ErrorCode? error)> ValidateBalanceOperationAsync(
        int userId, int currencyId, decimal amount, bool requirePositiveAmount = true)
    {
        // Проверка на отрицательную сумму
        if (requirePositiveAmount && amount <= 0)
            return (false, ErrorCode.TransactionAmountMustBePositive);

        // Проверка существования баланса
        var balance = await GetUserBalanceAsync(userId, currencyId);
        if (balance == null)
            return (false, ErrorCode.BalanceNotFound);

        // Проверка достаточности средств (только для снятия)
        if (amount > 0 && balance.Amount < amount)
            return (false, ErrorCode.TransactionInsufficientFunds);

        return (true, null);
    }

    public override async Task<(bool canDelete, ErrorCode? error)> ValidateDeleteAsync(int id)
    {
        // Базовая проверка существования
        var baseValidation = await base.ValidateDeleteAsync(id);
        if (!baseValidation.canDelete)
            return baseValidation;

        // Можно добавить дополнительные проверки, например, если баланс используется в транзакциях
        return (true, null);
    }
} 