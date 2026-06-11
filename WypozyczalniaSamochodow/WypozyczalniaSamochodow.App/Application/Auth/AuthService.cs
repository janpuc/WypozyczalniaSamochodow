using WypozyczalniaSamochodow.App.Application.Repositories;
using WypozyczalniaSamochodow.App.Domain.Users;

namespace WypozyczalniaSamochodow.App.Application.Auth;

internal sealed class AuthService
{
    private readonly IClientRepository _clients;
    private readonly IBackofficeRepository _backoffice;
    private readonly IPasswordHasher _hasher;

    public AuthService(IClientRepository clients, IBackofficeRepository backoffice, IPasswordHasher hasher)
    {
        _clients = clients;
        _backoffice = backoffice;
        _hasher = hasher;
    }

    public User? Login(string rawEmail, string plainPassword)
    {
        throw new NotImplementedException();
    }

    public RegistrationResult RegisterClient(string fullName, string rawEmail, string plainPassword, DrivingLicence? licence)
    {
        throw new NotImplementedException();
    }
}
