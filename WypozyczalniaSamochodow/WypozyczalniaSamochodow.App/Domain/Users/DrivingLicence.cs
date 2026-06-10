using WypozyczalniaSamochodow.App.Domain.Shared;

namespace WypozyczalniaSamochodow.App.Domain.Users;

internal sealed record DrivingLicence
{
    public string Number { get; }
    public DateOnly ExpiryDate { get; }

    public DrivingLicence(string number, DateOnly expiryDate)
    {
        if (string.IsNullOrWhiteSpace(number))
            throw new DomainException("Numer prawa jazdy nie może być pusty.");
        Number = number;
        ExpiryDate = expiryDate;
    }

    public bool IsValidOn(DateOnly date) => ExpiryDate >= date;
}
