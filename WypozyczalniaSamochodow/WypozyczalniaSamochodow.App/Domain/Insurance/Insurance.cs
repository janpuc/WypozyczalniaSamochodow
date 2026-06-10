using WypozyczalniaSamochodow.App.Domain.Shared;

namespace WypozyczalniaSamochodow.App.Domain.Insurance;

internal sealed class Insurance
{
    public string Company { get; }
    public PolicyNumber Number { get; }
    public string PolicyName { get; }
    public DateOnly IssueDate { get; }
    public DateOnly ExpiryDate { get; }
    public Money Cost { get; }

    public Insurance(string company, PolicyNumber number, string policyName,
        DateOnly issueDate, DateOnly expiryDate, Money cost)
    {
        if (string.IsNullOrWhiteSpace(company))
            throw new DomainException("Firma ubezpieczeniowa nie może być pusta.");
        if (string.IsNullOrWhiteSpace(policyName))
            throw new DomainException("Nazwa polisy nie może być pusta.");
        if (expiryDate <= issueDate)
            throw new DomainException("Data wygaśnięcia musi być po dacie wystawienia.");
        Company = company;
        Number = number;
        PolicyName = policyName;
        IssueDate = issueDate;
        ExpiryDate = expiryDate;
        Cost = cost;
    }

    public bool IsValidOn(DateOnly date) => IssueDate <= date && ExpiryDate >= date;

    public bool Covers(DateRange range) => IsValidOn(range.From) && IsValidOn(range.EffectiveTo);
}
