using Microsoft.EntityFrameworkCore;
using NekitCoinsManager.Core.Models;
using NekitCoinsManager.Core.Services;

namespace NekitCoinsManager.Core.Data;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Transaction> Transactions { get; set; } = null!;
    public DbSet<Currency> Currencies { get; set; } = null!;
    public DbSet<UserBalance> UserBalances { get; set; } = null!;
    public DbSet<UserAuthToken> AuthTokens { get; set; } = null!;
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string dbPath = Path.Combine(SettingsConstants.SettingsDirectory, "NekitCoins.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.FromUser)
            .WithMany(u => u.SentTransactions)
            .HasForeignKey(t => t.FromUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.ToUser)
            .WithMany(u => u.ReceivedTransactions)
            .HasForeignKey(t => t.ToUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Currency)
            .WithMany()
            .HasForeignKey(t => t.CurrencyId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserBalance>()
            .HasOne(ub => ub.User)
            .WithMany(u => u.Balances)
            .HasForeignKey(ub => ub.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserBalance>()
            .HasOne(ub => ub.Currency)
            .WithMany()
            .HasForeignKey(ub => ub.CurrencyId)
            .OnDelete(DeleteBehavior.Restrict);

        // Создаем уникальный индекс для комбинации UserId и CurrencyId
        modelBuilder.Entity<UserBalance>()
            .HasIndex(ub => new { ub.UserId, ub.CurrencyId })
            .IsUnique();

        modelBuilder.Entity<UserAuthToken>()
            .HasOne(t => t.User)
            .WithMany()
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Создаем уникальный индекс для токена
        modelBuilder.Entity<UserAuthToken>()
            .HasIndex(t => t.Token)
            .IsUnique();
        
        // Инициализация стартовых значений
        SeedInitialData(modelBuilder);
    }

    private void SeedInitialData(ModelBuilder modelBuilder)
    {
        // Добавляем базовую валюту NekitCoins
        var nekitCoinsCurrency = new Currency
        {
            Id = 1,
            Name = "NekitCoins",
            Code = "NKC",
            Symbol = "₦",
            ExchangeRate = 1m, // Базовый курс
            LastUpdateTime = DateTime.UtcNow,
            IsActive = true,
            IsDefaultForNewUsers = true, // Это валюта по умолчанию для новых пользователей
            DefaultAmount = 100m // Начальное количество для новых пользователей
        };
        
        modelBuilder.Entity<Currency>().HasData(nekitCoinsCurrency);

        // Создаем банковский аккаунт
        var bankUser = new User
        {
            Id = 1,
            Username = "РОФЛОБАНК",
            // Хешируем стандартный пароль для безопасности
            PasswordHash = new PasswordHasherService().HashPassword("Bank@BankAccount!"),
            CreatedAt = DateTime.UtcNow,
            IsBankAccount = true // Устанавливаем признак банковского аккаунта
        };
        
        modelBuilder.Entity<User>().HasData(bankUser);

        // Добавляем баланс банка - 500,000 NekitCoins
        var bankBalance = new UserBalance
        {
            Id = 1,
            UserId = bankUser.Id,
            CurrencyId = nekitCoinsCurrency.Id,
            Amount = 500000m,
            LastUpdateTime = DateTime.UtcNow
        };
        
        modelBuilder.Entity<UserBalance>().HasData(bankBalance);
    }
} 