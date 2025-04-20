using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NekitCoinsManager.Core.Data;
using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Core.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public override async Task<IEnumerable<User>> GetAllAsync()
    {
        return await DbSet
            .Include(u => u.Balances)
            .ToListAsync();
    }

    public override async Task<User?> GetByIdAsync(int id)
    {
        return await DbSet
            .Include(u => u.Balances)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public override async Task<IEnumerable<User>> FindAsync(Expression<Func<User, bool>> predicate)
    {
        return await DbSet
            .Include(u => u.Balances)
            .Where(predicate)
            .ToListAsync();
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await DbSet
            .Include(u => u.Balances)
            .FirstOrDefaultAsync(u => u.Username.Equals(username));
    }

    public async Task<bool> IsUsernameUniqueAsync(string username, int? excludeId = null)
    {
        return !await DbSet
            .AnyAsync(u => u.Username.Equals(username) && (!excludeId.HasValue || u.Id != excludeId.Value));
    }

    public async Task<User?> GetBankAccountAsync()
    {
        return await DbSet
            .Include(u => u.Balances)
            .FirstOrDefaultAsync(u => u.IsBankAccount);
    }

    // Реализация методов валидации
    public override async Task<(bool isValid, ErrorCode? error)> ValidateEntityAsync(User entity)
    {
        // Проверка обязательных полей
        if (string.IsNullOrWhiteSpace(entity.Username))
            return (false, ErrorCode.UserUsernameEmpty);

        if (string.IsNullOrWhiteSpace(entity.PasswordHash))
            return (false, ErrorCode.UserPasswordHashEmpty);

        // Проверка формата имени пользователя
        if (entity.Username.Length < 3)
            return (false, ErrorCode.UserUsernameTooShort);

        if (entity.Username.Length > 50)
            return (false, ErrorCode.UserUsernameTooLong);

        // Проверка на недопустимые символы в имени пользователя
        // Разрешаем только буквы, цифры и символы подчеркивания в имени пользователя
        if (!System.Text.RegularExpressions.Regex.IsMatch(entity.Username, @"^[a-zA-Z0-9_]+$"))
            return (false, ErrorCode.UserUsernameInvalidCharacters);

        return (true, null);
    }

    public override async Task<(bool isValid, ErrorCode? error)> ValidateCreateAsync(User entity)
    {
        // Базовая валидация сущности
        var baseValidation = await ValidateEntityAsync(entity);
        if (!baseValidation.isValid)
            return baseValidation;

        // Проверка на уникальность имени пользователя
        var isUnique = await IsUsernameUniqueAsync(entity.Username);
        if (!isUnique)
            return (false, ErrorCode.UserUsernameAlreadyExists);

        return (true, null);
    }

    public override async Task<(bool isValid, ErrorCode? error)> ValidateUpdateAsync(User entity)
    {
        // Проверка существования пользователя
        var existingUser = await GetByIdAsync(entity.Id);
        if (existingUser == null)
            return (false, ErrorCode.UserNotFound);

        // Если имя пользователя не менялось, пропускаем проверку уникальности
        if (entity.Username != existingUser.Username)
        {
            // Проверка на уникальность имени пользователя
            var isUnique = await IsUsernameUniqueAsync(entity.Username, entity.Id);
            if (!isUnique)
                return (false, ErrorCode.UserUsernameAlreadyExists);
        }

        return (true, null);
    }

    public override async Task<(bool canDelete, ErrorCode? error)> ValidateDeleteAsync(int id)
    {
        // Базовая проверка существования
        var baseValidation = await base.ValidateDeleteAsync(id);
        if (!baseValidation.canDelete)
            return baseValidation;

        // Проверка, что пользователь не является банковским аккаунтом
        var user = await GetByIdAsync(id);
        if (user!.IsBankAccount)
            return (false, ErrorCode.UserCannotDeleteBankAccount);

        // Проверка наличия балансов у пользователя
        if (user.Balances.Any())
            return (false, ErrorCode.UserHasBalances);

        return (true, null);
    }

    public async Task<(bool isValid, ErrorCode? error)> ValidatePasswordAsync(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return (false, ErrorCode.UserPasswordEmpty);

        if (password.Length < 6)
            return (false, ErrorCode.UserPasswordTooShort);

        if (password.Length > 100)
            return (false, ErrorCode.UserPasswordTooLong);

        // Проверка сложности пароля
        bool hasUpperCase = password.Any(char.IsUpper);
        bool hasLowerCase = password.Any(char.IsLower);
        bool hasDigit = password.Any(char.IsDigit);
        bool hasSpecialChar = password.Any(c => !char.IsLetterOrDigit(c));

        if (!(hasUpperCase && hasLowerCase && hasDigit))
            return (false, ErrorCode.UserPasswordNotComplex);

        return (true, null);
    }
} 