using System.Security.Cryptography;
using Findly.Application.Common.Interfaces;

namespace Findly.Infrastructure.Security;

/// <summary>
/// PBKDF2 (SHA-256) password hashing. Stored format: "{iterations}.{saltBase64}.{hashBase64}".
/// The admin seed script (009_SeedAdminUser.sql) contains a hash precomputed with these exact parameters.
/// </summary>
internal sealed class PasswordHasher : IPasswordHasher
{
    private const int Iterations = 100_000;
    private const int SaltSize   = 16;
    private const int KeySize    = 32;

    public string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var key  = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, KeySize);

        return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
    }

    public bool Verify(string password, string storedHash)
    {
        var parts = storedHash.Split('.', 3);
        if (parts.Length != 3 || !int.TryParse(parts[0], out var iterations))
            return false;

        byte[] salt, expected;
        try
        {
            salt     = Convert.FromBase64String(parts[1]);
            expected = Convert.FromBase64String(parts[2]);
        }
        catch (FormatException)
        {
            return false;
        }

        var actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expected.Length);
        return CryptographicOperations.FixedTimeEquals(actual, expected);
    }
}
