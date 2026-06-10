using WypozyczalniaSamochodow.App.Domain.Users;

namespace WypozyczalniaSamochodow.App.Application.Repositories;

internal interface IUserRepository<T> where T : User
{
    IReadOnlyList<T> All { get; }
    T? FindByEmail(Email email);
    void Add(T user);
    void Remove(T user);
    bool IsEmailTaken(Email email);
}
