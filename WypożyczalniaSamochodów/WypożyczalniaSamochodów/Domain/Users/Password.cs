using OrlenSolutions.Domain.Shared;

namespace OrlenSolutions.Domain.Users;

internal sealed class Password
{
    public string Hash { get; }

    private Password(string hash) { Hash = hash; }

    public static Password FromPlain(string plain, IPasswordHasher hasher)
    {
        if (string.IsNullOrWhiteSpace(plain))
            throw new DomainException("Hasło nie może być puste.");
        return new Password(hasher.Hash(plain));
    }

    public static Password FromHash(string hash)
    {
        if (string.IsNullOrWhiteSpace(hash))
            throw new DomainException("Hash hasła nie może być pusty.");
        return new Password(hash);
    }

    public bool Verify(string plain, IPasswordHasher hasher) => hasher.Verify(plain, Hash);
}
