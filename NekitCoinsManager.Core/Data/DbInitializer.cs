using System;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace NekitCoinsManager.Core.Data;

public static class DbInitializer
{
    private const string DbFileName = "Data/NekitCoins.db";
    
    /// <summary>
    /// Проверяет и инициализирует базу данных
    /// </summary>
    public static void Initialize(AppDbContext context, bool forceReset = false)
    {
        if (forceReset)
        {
            // Удаляем существующую базу данных
            context.Database.EnsureDeleted();
        }
        
        // Проверяем существование базы данных
        bool created = context.Database.EnsureCreated();
        
        // Если база данных уже существует, проверяем наличие данных
        if (!created && !forceReset)
        {
            // Проверяем наличие валюты NekitCoins
            bool hasNekitCoins = context.Currencies.Any(c => c.Code == "NKC");
            
            // Проверяем наличие аккаунта банка
            bool hasBankAccount = context.Users.Any(u => u.Username == "BankAccount" && u.IsBankAccount);
            
            // Если основные данные отсутствуют, пересоздаем базу
            if (!hasNekitCoins || !hasBankAccount)
            {
                // Закрываем подключение к БД
                context.Database.EnsureDeleted();
                
                // Удаляем файл базы данных
                string dbPath = Path.Combine(Environment.CurrentDirectory, DbFileName);
                if (File.Exists(dbPath))
                {
                    File.Delete(dbPath);
                }
                
                // Создаем новую БД
                using var newContext = new AppDbContext();
                newContext.Database.EnsureCreated();
            }
        }
    }
} 