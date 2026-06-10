using WypozyczalniaSamochodow.App.Domain.Shared;

namespace WypozyczalniaSamochodow.App.Domain.Payments;

internal sealed class BankTransferPayment : Payment
{
    public string Iban { get; }
    public BankTransferPayment(Money amount, DateTime paidAt, string iban) : base(amount, paidAt)
    {
        if (string.IsNullOrWhiteSpace(iban))
            throw new DomainException("IBAN nie może być pusty.");
        Iban = iban.Trim();
    }
    public override string MethodName => "Przelew bankowy";
    public override string Describe() => $"Przelew ({Iban})";
}
