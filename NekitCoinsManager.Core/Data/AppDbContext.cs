using Microsoft.EntityFrameworkCore;
using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Core.Data;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Transaction> Transactions { get; set; } = null!;

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
    }
} 