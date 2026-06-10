using WypozyczalniaSamochodow.App.Domain.Shared;

namespace WypozyczalniaSamochodow.App.Domain.Users;

internal sealed class Client : User
{
    public DrivingLicence? DrivingLicence { get; private set; }

    public Client(string fullName, Email email, Password password, DrivingLicence? licence = null)
        : base(fullName, email, password)
    {
    }

    public void RegisterLicence(DrivingLicence licence) => DrivingLicence = licence;
    public void RemoveLicence() => DrivingLicence = null;

    public void EnsureCanRent(IClock clock)
    {

    }
}

