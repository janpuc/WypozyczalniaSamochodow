using WypozyczalniaSamochodow.App.Domain.Users;

namespace WypozyczalniaSamochodow.App.Infrastructure.Persistence;

internal abstract class InMemoryUserRepository<T> : InMemoryRepository<T> where T : User
{
    public T? FindByEmail(Email email) => Items.FirstOrDefault(u => u.Email.Equals(email));

    public bool IsEmailTaken(Email email) => Items.Any(u => u.Email.Equals(email));
}
