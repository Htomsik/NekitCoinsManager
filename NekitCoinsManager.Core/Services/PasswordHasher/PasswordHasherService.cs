using System;
using Konscious.Security.Cryptography;
using System.Text;
using System.Security.Cryptography;

namespace NekitCoinsManager.Core.Services;

public class PasswordHasherService : IPasswordHasherService
{
    private const int SaltSize = 16; // размер соли в байтах
    private const int HashSize = 32; // размер хеша в байтах
    private const int DegreeOfParallelism = 8; // степень параллелизма
    private const int Iterations = 4; // количество итераций
    private const int MemorySize = 1024 * 1024; // размер памяти (1 GB)

    public string HashPassword(string password)
    {
        // Генерируем соль
        byte[] salt = new byte[SaltSize];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        // Создаем экземпляр Argon2id
        using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            DegreeOfParallelism = DegreeOfParallelism,
            Iterations = Iterations,
            MemorySize = MemorySize
        };

        // Получаем хеш
        byte[] hash = argon2.GetBytes(HashSize);

        // Комбинируем соль и хеш
        byte[] hashBytes = new byte[SaltSize + HashSize];
        Array.Copy(salt, 0, hashBytes, 0, SaltSize);
        Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

        // Конвертируем в Base64 для хранения
        return Convert.ToBase64String(hashBytes);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        try
        {
            // Декодируем из Base64
            byte[] hashBytes = Convert.FromBase64String(hashedPassword);

            // Извлекаем соль
            byte[] salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            // Извлекаем оригинальный хеш
            byte[] originalHash = new byte[HashSize];
            Array.Copy(hashBytes, SaltSize, originalHash, 0, HashSize);

            // Создаем экземпляр Argon2id с той же солью и параметрами
            using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                DegreeOfParallelism = DegreeOfParallelism,
                Iterations = Iterations,
                MemorySize = MemorySize
            };

            // Вычисляем хеш для проверки
            byte[] newHash = argon2.GetBytes(HashSize);

            // Сравниваем хеши
            return CryptographicOperations.FixedTimeEquals(originalHash, newHash);
        }
        catch
        {
            return false;
        }
    }
} 