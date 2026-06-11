namespace WypozyczalniaSamochodow.App.Infrastructure.Security;

internal static class Argon2Policy
{
    public const int TimeCost = 3;
    public const int MemoryCost = 65536;
    public const int Lanes = 4;
    public const int Threads = 4;
    public const int HashLength = 32;
    public const int SaltSize = 16;
}
