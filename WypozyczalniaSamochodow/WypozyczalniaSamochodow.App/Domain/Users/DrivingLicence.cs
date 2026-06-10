using WypozyczalniaSamochodow.App.Domain.Shared;

namespace WypozyczalniaSamochodow.App.Domain.Users;

internal sealed record DrivingLicence
{
    public string Number { get; }
    public DateOnly ExpiryDate { get; }

    public DrivingLicence(string number, DateOnly expiryDate)
    {

    }

    public bool IsValidOn(DateOnly date) => ExpiryDate >= date;
}
