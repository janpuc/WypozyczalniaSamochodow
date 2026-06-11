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
        EnsureEmailAvailable(email);
        EnsurePasswordMeetsPolicy(plainPassword);
        var client = new Client(fullName, email, Password.FromPlain(plainPassword, _hasher), licence);
        _clients.Add(client);
        return client;
    }

    public Backoffice CreateBackofficeUser(string fullName, Email email, string plainPassword)
    {
        EnsureEmailAvailable(email);
        EnsurePasswordMeetsPolicy(plainPassword);
        var user = new Backoffice(fullName, email, Password.FromPlain(plainPassword, _hasher));
        _backoffice.Add(user);
        return user;
    }

    public void ResetPassword(User user, string plainPassword)
    {
        EnsurePasswordMeetsPolicy(plainPassword);
        user.ResetPassword(plainPassword, _hasher);
    }

    public void UpdateProfile(User user, string fullName, string email)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new DomainException("Imię i nazwisko nie może być puste.");
        var newEmail = new Email(email);
        if (!newEmail.Equals(user.Email))
            EnsureEmailAvailable(newEmail);
        user.Rename(fullName);
        user.ChangeEmail(newEmail);
    }

    public void RegisterLicence(Client client, DrivingLicence licence) => client.RegisterLicence(licence);

    public void RemoveClient(Client client)
    {
        if (_reservations.HasActiveOf(client))
            throw new DomainException("Nie można usunąć klienta z aktywną rezerwacją.");
        _clients.Remove(client);
    }

    public void RemoveBackofficeUser(Backoffice user) => _backoffice.Remove(user);

    private void EnsureEmailAvailable(Email email)
    {
        if (_clients.IsEmailTaken(email) || _backoffice.IsEmailTaken(email))
            throw new DomainException("Użytkownik z tym emailem już istnieje.");
    }

    private static void EnsurePasswordMeetsPolicy(string plainPassword)
    {
        if (!PasswordPolicy.IsSatisfiedBy(plainPassword))
            throw new DomainException($"Hasło musi mieć co najmniej {PasswordPolicy.MinimumLength} znaków.");
    }
}

