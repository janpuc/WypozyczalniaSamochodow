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
        if (!Email.TryCreate(rawEmail, out var email))
            return null;

        User? user = _clients.FindByEmail(email);
        user ??= _backoffice.FindByEmail(email);
        if (user is null) return null;
        return user.Password.Verify(plainPassword, _hasher) ? user : null;
    }

    public RegistrationResult RegisterClient(string fullName, string rawEmail, string plainPassword, DrivingLicence? licence)
    {
        if (!Email.TryCreate(rawEmail, out var email))
            return RegistrationResult.InvalidEmail;
        if (!PasswordPolicy.IsSatisfiedBy(plainPassword))
            return RegistrationResult.WeakPassword;
        if (_clients.IsEmailTaken(email) || _backoffice.IsEmailTaken(email))
            return RegistrationResult.EmailTaken;

        var password = Password.FromPlain(plainPassword, _hasher);
        _clients.Add(new Client(fullName, email, password, licence));
        return RegistrationResult.Success;
    }
}
