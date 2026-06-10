namespace OrlenSolutions.Domain.Users;

internal interface IPasswordHasher
{
    string Hash(string plain);
    bool Verify(string plain, string hash);
}
