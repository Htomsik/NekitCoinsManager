using System;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NekitCoinsManager.Core.Models;
using NekitCoinsManager.Core.Services;

namespace NekitCoinsManager.Core.Data;

public static class DbInitializer
{
    private const string DbFileName = "NekitCoins.db";
    
    /// <summary>
    /// Проверяет и инициализирует базу данных
    /// </summary>
    public static void Initialize(AppDbContext context, bool forceReset = false)
    {
        try
        {
            // Создаем директорию для базы данных, если она не существует
            Directory.CreateDirectory(SettingsConstants.SettingsDirectory);
            
            if (forceReset)
            {
                // Удаляем существующую базу данных только при явном указании
                context.Database.EnsureDeleted();
            }
            
            // Проверяем/создаем базу данных
            bool created = context.Database.EnsureCreated();
            
            // Если база данных только что создана, то данные уже заполнены через SeedInitialData
            if (created)
            {
                Console.WriteLine("База данных успешно создана и инициализирована.");
                return;
            }
            
            // Если база данных существует, проверяем только критические данные
            if (!forceReset)
            {
                bool hasBankAccount = context.Users.Any(u => u.IsBankAccount);
                bool hasNekitCoins = context.Currencies.Any(c => c.Code == "NKC");
                
                // Если критически важные данные отсутствуют, восстанавливаем только их
                if (!hasBankAccount)
                {
                    // Создаем банковский аккаунт, если он отсутствует
                    var bankUser = new NekitCoinsManager.Core.Models.User
                    {
                        Username = "РОФЛОБАНК",
                        PasswordHash = new PasswordHasherService().HashPassword("Bank@BankAccount!"),
                        CreatedAt = DateTime.UtcNow,
                        IsBankAccount = true
                    };
                    
                    context.Users.Add(bankUser);
                    context.SaveChanges();
                    
                    Console.WriteLine("Банковский аккаунт восстановлен.");
                }
                
                if (!hasNekitCoins)
                {
                    // Создаем валюту NekitCoins, если она отсутствует
                    var nekitCoinsCurrency = new NekitCoinsManager.Core.Models.Currency
                    {
                        Name = "NekitCoins",
                        Code = "NKC",
                        Symbol = "₦",
                        ExchangeRate = 1m,
                        LastUpdateTime = DateTime.UtcNow,
                        IsActive = true,
                        IsDefaultForNewUsers = true,
                        DefaultAmount = 100m
                    };
                    
                    context.Currencies.Add(nekitCoinsCurrency);
                    context.SaveChanges();
                    
                    Console.WriteLine("Валюта NekitCoins восстановлена.");
                }
                
                // Проверяем, есть ли у банка баланс NekitCoins
                var bankUser2 = context.Users.FirstOrDefault(u => u.IsBankAccount);
                var nekitCoinsCurrency2 = context.Currencies.FirstOrDefault(c => c.Code == "NKC");
                
                if (bankUser2 != null && nekitCoinsCurrency2 != null)
                {
                    bool hasBankBalance = context.UserBalances.Any(b => 
                        b.UserId == bankUser2.Id && b.CurrencyId == nekitCoinsCurrency2.Id);
                    
                    if (!hasBankBalance)
                    {
                        // Если у банка нет баланса, создаем его с начальным значением
                        var bankBalance = new NekitCoinsManager.Core.Models.UserBalance
                        {
                            UserId = bankUser2.Id,
                            CurrencyId = nekitCoinsCurrency2.Id,
                            Amount = 500000m,
                            LastUpdateTime = DateTime.UtcNow
                        };
                        
                        context.UserBalances.Add(bankBalance);
                        context.SaveChanges();
                        
                        Console.WriteLine("Баланс банка восстановлен.");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при инициализации базы данных: {ex.Message}");
        }
    }
} 