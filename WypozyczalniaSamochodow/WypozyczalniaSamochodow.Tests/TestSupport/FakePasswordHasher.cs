using WypozyczalniaSamochodow.App.Domain.Users;

namespace WypozyczalniaSamochodow.Tests.TestSupport;

internal sealed class FakePasswordHasher : IPasswordHasher
{
    public string Hash(string plain) => $"hash:{plain}";

    public bool Verify(string plain, string hash) => hash == $"hash:{plain}";
}
