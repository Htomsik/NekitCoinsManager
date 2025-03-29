using System;
using System.Text;
using System.Security.Cryptography;

namespace NekitCoinsManager.Core.Services;

public class PasswordHasherService : IPasswordHasherService
{
    private const int SaltSize = 16; // размер соли в байтах

    public string HashPassword(string password)
    {
        // Генерируем соль
        byte[] salt = GenerateSalt();

        // Вычисляем хеш пароля с солью
        byte[] hash = ComputeHash(password, salt);

        // Комбинируем соль и хеш
        byte[] hashBytes = CombineArrays(salt, hash);

        // Конвертируем в Base64 для хранения
        return Convert.ToBase64String(hashBytes);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        try
        {
            // Декодируем из Base64
            byte[] hashBytes = Convert.FromBase64String(hashedPassword);

            // Извлекаем соль и хеш
            var (salt, originalHash) = ExtractSaltAndHash(hashBytes);

            // Вычисляем хеш для проверки
            byte[] newHash = ComputeHash(password, salt);

            // Сравниваем хеши
            return CryptographicOperations.FixedTimeEquals(originalHash, newHash);
        }
        catch
        {
            return false;
        }
    }

    private byte[] GenerateSalt()
    {
        byte[] salt = new byte[SaltSize];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }
        return salt;
    }

    private byte[] ComputeHash(string password, byte[] salt)
    {
        // Комбинируем пароль и соль
        byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
        byte[] combinedBytes = CombineArrays(passwordBytes, salt);

        // Вычисляем хеш
        using (var sha256 = SHA256.Create())
        {
            return sha256.ComputeHash(combinedBytes);
        }
    }

    private byte[] CombineArrays(byte[] first, byte[] second)
    {
        byte[] combined = new byte[first.Length + second.Length];
        Array.Copy(first, 0, combined, 0, first.Length);
        Array.Copy(second, 0, combined, first.Length, second.Length);
        return combined;
    }

    private (byte[] salt, byte[] hash) ExtractSaltAndHash(byte[] hashBytes)
    {
        // Извлекаем соль
        byte[] salt = new byte[SaltSize];
        Array.Copy(hashBytes, 0, salt, 0, SaltSize);

        // Извлекаем хеш
        byte[] hash = new byte[hashBytes.Length - SaltSize];
        Array.Copy(hashBytes, SaltSize, hash, 0, hash.Length);

        return (salt, hash);
    }
} 