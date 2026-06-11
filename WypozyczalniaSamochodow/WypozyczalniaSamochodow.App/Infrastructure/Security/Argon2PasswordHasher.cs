using WypozyczalniaSamochodow.App.Domain.Users;

namespace WypozyczalniaSamochodow.App.Infrastructure.Security;

internal sealed class Argon2PasswordHasher : IPasswordHasher
{
    public string Hash(string plain)
    {
        throw new NotImplementedException();
    }

    public bool Verify(string plain, string hash)
    {
        throw new NotImplementedException();
    }

    private static byte[] GenerateSalt()
    {
        throw new NotImplementedException();
    }
}
