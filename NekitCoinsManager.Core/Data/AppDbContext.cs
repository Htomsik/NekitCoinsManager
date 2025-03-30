using Microsoft.EntityFrameworkCore;
using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Core.Data;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Transaction> Transactions { get; set; } = null!;
    public DbSet<Currency> Currencies { get; set; } = null!;
    public DbSet<UserBalance> UserBalances { get; set; } = null!;

    public AppDbContext()
    {
        // Создаем БД при первом запуске, если она не существует
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=NekitCoins.db");
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
    }
} 