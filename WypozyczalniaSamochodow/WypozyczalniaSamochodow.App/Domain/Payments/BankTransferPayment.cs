using WypozyczalniaSamochodow.App.Domain.Shared;

namespace WypozyczalniaSamochodow.App.Domain.Payments;

internal sealed class BankTransferPayment : Payment
{
    public string Iban { get; }
    public BankTransferPayment(Money amount, DateTime paidAt, string iban) : base(amount, paidAt)
    {

    }
    public override string MethodName => "Przelew bankowy";
    public override string Describe() => $"Przelew ({Iban})";
}
