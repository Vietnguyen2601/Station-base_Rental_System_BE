using System.Security.Cryptography;
using System.Text;

namespace EVStationRental.Common.Helpers;

public static class PasswordHasher
{
    private const int Iterations = 100_000;
    private const int SaltSize = 16;
    private const int KeySize = 32;

    public static string Hash(string password)
    {
        using var rng = RandomNumberGenerator.Create();
        var salt = new byte[SaltSize];
        rng.GetBytes(salt);

        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
        var key = pbkdf2.GetBytes(KeySize);
        return $"PBKDF2$SHA256${Iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(key)}";
    }

    public static bool Verify(string password, string hash)
    {
        // Legacy plaintext fallback
        if (!hash.Contains("$"))
        {
            return password == hash;
        }

        var parts = hash.Split('$');
        if (parts.Length != 5 || parts[0] != "PBKDF2") return false;
        var algo = parts[1];
        var iterations = int.Parse(parts[2]);
        var salt = Convert.FromBase64String(parts[3]);
        var key = Convert.FromBase64String(parts[4]);

        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
        var computed = pbkdf2.GetBytes(key.Length);
        return CryptographicOperations.FixedTimeEquals(computed, key);
    }
}


