using WypozyczalniaSamochodow.App.Domain.Shared;

namespace WypozyczalniaSamochodow.App.Domain.Users;

internal abstract class User
{
    public string FullName { get; private set; }
    public Email Email { get; private set; }
    public Password Password { get; private set; }

    protected User(string fullName, Email email, Password password)
    {

    }

    public void Rename(string fullName)
    {

    }

    public void ChangeEmail(Email email) => Email = email;

    public void ResetPassword(string newPlain, IPasswordHasher hasher) =>
        Password = Password.FromPlain(newPlain, hasher);
}

