using Microsoft.EntityFrameworkCore;
using NekitCoinsManager.Core.Data;
using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Core.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _dbContext;
    private readonly IPasswordHasherService _passwordHasherService;
    private readonly IUserBalanceService _userBalanceService;
    private readonly ICurrencyService _currencyService;

    public UserService(
        AppDbContext dbContext, 
        IPasswordHasherService passwordHasherService,
        IUserBalanceService userBalanceService,
        ICurrencyService currencyService)
    {
        _dbContext = dbContext;
        _passwordHasherService = passwordHasherService;
        _userBalanceService = userBalanceService;
        _currencyService = currencyService;
    }

    public async Task<IEnumerable<User>> GetUsersAsync()
    {
        return await _dbContext.Users.ToListAsync();
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Username.Equals(username));
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<(bool success, string? error)> AddUserAsync(string username, string password, string confirmPassword)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return (false, "Имя пользователя не может быть пустым");
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            return (false, "Пароль не может быть пустым");
        }

        if (password != confirmPassword)
        {
            return (false, "Пароли не совпадают");
        }

        var existingUser = await GetUserByUsernameAsync(username);
        if (existingUser != null)
        {
            return (false, "Пользователь с таким именем уже существует");
        }

        var user = new User
        {
            Username = username,
            PasswordHash = _passwordHasherService.HashPassword(password),
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        // Получаем валюту с самым низким коэффициентом обмена
        var currencies = await _currencyService.GetCurrenciesAsync();
        var lowestRateCurrency = currencies.OrderBy(c => c.ExchangeRate).FirstOrDefault();

        if (lowestRateCurrency != null)
        {
            // Создаем начальный баланс в валюте с самым низким коэффициентом
            var (success, error) = await _userBalanceService.CreateBalanceAsync(
                user.Id,
                lowestRateCurrency.Id,
                100 // Начальный баланс
            );

            if (!success)
            {
                // Если не удалось создать баланс, удаляем пользователя
                _dbContext.Users.Remove(user);
                await _dbContext.SaveChangesAsync();
                return (false, error ?? "Не удалось создать начальный баланс");
            }
        }

        return (true, null);
    }

    public async Task<(bool success, string? error)> DeleteUserAsync(int userId)
    {
        var user = await _dbContext.Users
            .Include(u => u.SentTransactions)
            .Include(u => u.ReceivedTransactions)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return (false, "Пользователь не найден");
        }

        if (user.SentTransactions.Any() || user.ReceivedTransactions.Any())
        {
            return (false, "Невозможно удалить пользователя с историей транзакций");
        }

        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();
        
        return (true, null);
    }
} 