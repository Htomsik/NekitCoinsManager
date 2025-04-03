using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NekitCoinsManager.Core.Data;
using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Core.Repositories;

public class TransactionRepository : BaseRepository<Transaction>, ITransactionRepository
{
    public TransactionRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public override async Task<IEnumerable<Transaction>> GetAllAsync()
    {
        return await DbSet
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public override async Task<Transaction?> GetByIdAsync(int id)
    {
        return await DbSet
            .Include(t => t.Currency)
            .Include(t => t.FromUser)
            .Include(t => t.ToUser)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public override async Task<IEnumerable<Transaction>> FindAsync(Expression<Func<Transaction, bool>> predicate)
    {
        return await DbSet
            .Where(predicate)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> HasTransactionsWithCurrencyAsync(int currencyId)
    {
        return await DbSet
            .AnyAsync(t => t.CurrencyId == currencyId);
    }
    
   

    // Реализация методов валидации
    public override async Task<(bool isValid, ErrorCode? error)> ValidateEntityAsync(Transaction entity)
    {
        // Проверка обязательных полей
        if (entity.FromUserId <= 0)
            return (false, ErrorCode.TransactionFromUserIdInvalid);

        if (entity.ToUserId <= 0)
            return (false, ErrorCode.TransactionToUserIdInvalid);

        if (entity.CurrencyId <= 0)
            return (false, ErrorCode.TransactionCurrencyIdInvalid);

        if (entity.Amount <= 0)
            return (false, ErrorCode.TransactionAmountMustBePositive);

        // Проверка допустимости типа транзакции
        if (!Enum.IsDefined(typeof(TransactionType), entity.Type))
            return (false, ErrorCode.TransactionInvalidType);

        // Проверка, что пользователь не переводит деньги самому себе (кроме конвертации)
        if (entity.FromUserId == entity.ToUserId && entity.Type != TransactionType.Conversion)
            return (false, ErrorCode.TransactionSelfTransactionNotAllowed);

        // Валидация для депозита
        if (entity.Type == TransactionType.Deposit && entity.FromUserId != 1) // Предполагается, что банк имеет ID=1
            return (false, ErrorCode.TransactionDepositFromNonBank);

        return (true, null);
    }

    public override async Task<(bool isValid, ErrorCode? error)> ValidateCreateAsync(Transaction entity)
    {
        // Базовая валидация сущности
        var baseValidation = await ValidateEntityAsync(entity);
        if (!baseValidation.isValid)
            return baseValidation;

        // Проверка родительской транзакции, если указана
        if (entity.ParentTransactionId.HasValue)
        {
            var parentExists = await ExistsByIdAsync(entity.ParentTransactionId.Value);
            if (!parentExists)
                return (false, ErrorCode.TransactionParentNotFound);
        }

        return (true, null);
    }

    public override async Task<(bool isValid, ErrorCode? error)> ValidateUpdateAsync(Transaction entity)
    {
        // Проверка существования транзакции
        var existingTransaction = await GetByIdAsync(entity.Id);
        if (existingTransaction == null)
            return (false, ErrorCode.TransactionNotFound);

        // Транзакции обычно не должны изменяться после создания
        // Но если нужно разрешить изменение комментария, это можно сделать здесь
        return (false, ErrorCode.TransactionCannotBeModified);
    }

    public override async Task<(bool canDelete, ErrorCode? error)> ValidateDeleteAsync(int id)
    {
        // Базовая проверка существования
        var baseValidation = await base.ValidateDeleteAsync(id);
        if (!baseValidation.canDelete)
            return baseValidation;

        // Проверка наличия дочерних транзакций
        var transaction = await GetByIdAsync(id);
        var hasChildTransactions = await DbSet.AnyAsync(t => t.ParentTransactionId == id);
        
        if (hasChildTransactions)
            return (false, ErrorCode.TransactionHasChildTransactions);

        // Транзакции обычно не удаляются в финансовых системах
        return (false, ErrorCode.TransactionCannotBeDeleted);
    }
} 