using WypozyczalniaSamochodow.App.Application.Auth;
using WypozyczalniaSamochodow.App.Application.Repositories;
using WypozyczalniaSamochodow.App.Domain.Shared;
using WypozyczalniaSamochodow.App.Domain.Users;

namespace WypozyczalniaSamochodow.App.Application.Users;

internal sealed class UserAccountService
{
    private readonly IClientRepository _clients;
    private readonly IBackofficeRepository _backoffice;
    private readonly IReservationRepository _reservations;
    private readonly IPasswordHasher _hasher;

    public UserAccountService(IClientRepository clients, IBackofficeRepository backoffice,
        IReservationRepository reservations, IPasswordHasher hasher)
    {
        _clients = clients;
        _backoffice = backoffice;
        _reservations = reservations;
        _hasher = hasher;
    }

    public Client CreateClient(string fullName, Email email, string plainPassword, DrivingLicence? licence)
    {
        throw new NotImplementedException();
    }

    public Backoffice CreateBackofficeUser(string fullName, Email email, string plainPassword)
    {
        throw new NotImplementedException();
    }

    public void ResetPassword(User user, string plainPassword)
    {
        throw new NotImplementedException();
    }

    public void UpdateProfile(User user, string fullName, string email)
    {
        throw new NotImplementedException();
    }

    public void RegisterLicence(Client client, DrivingLicence licence) => client.RegisterLicence(licence);

    public void RemoveClient(Client client)
    {
        throw new NotImplementedException();
    }

    public void RemoveBackofficeUser(Backoffice user) => throw new NotImplementedException();

    private void EnsureEmailAvailable(Email email)
    {
        throw new NotImplementedException();
    }

    private static void EnsurePasswordMeetsPolicy(string plainPassword)
    {
        throw new NotImplementedException();
    }
}

