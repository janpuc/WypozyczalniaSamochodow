namespace WypozyczalniaSamochodow.App.Domain.Users;

internal sealed class Backoffice : User
{
    public Backoffice(string fullName, Email email, Password password)
        : base(fullName, email, password) { }
}
