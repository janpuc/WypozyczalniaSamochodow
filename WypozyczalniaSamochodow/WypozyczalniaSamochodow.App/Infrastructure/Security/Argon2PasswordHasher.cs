using System.Security.Cryptography;
using System.Text;
using Isopoh.Cryptography.Argon2;
using WypozyczalniaSamochodow.App.Domain.Users;

namespace WypozyczalniaSamochodow.App.Infrastructure.Security;

internal sealed class Argon2PasswordHasher : IPasswordHasher
{
    public string Hash(string plain)
    {
        var config = new Argon2Config
        {
            Type = Argon2Type.HybridAddressing,
            Version = Argon2Version.Nineteen,
            TimeCost = Argon2Policy.TimeCost,
            MemoryCost = Argon2Policy.MemoryCost,
            Lanes = Argon2Policy.Lanes,
            Threads = Argon2Policy.Threads,
            Salt = GenerateSalt(),
            HashLength = Argon2Policy.HashLength,
            Password = Encoding.UTF8.GetBytes(plain)
        };
        var argon2 = new Argon2(config);
        using var hash = argon2.Hash();
        return config.EncodeString(hash.Buffer);
    }

    public bool Verify(string plain, string hash)
    {
        try
        {
            return Argon2.Verify(hash, plain);
        }
        catch (ArgumentException)
        {
            return false;
        }
    }

    private static byte[] GenerateSalt()
    {
        var salt = new byte[Argon2Policy.SaltSize];
        RandomNumberGenerator.Fill(salt);
        return salt;
    }
}
