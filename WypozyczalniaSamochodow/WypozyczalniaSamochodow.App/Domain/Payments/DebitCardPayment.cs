using WypozyczalniaSamochodow.App.Domain.Shared;

namespace WypozyczalniaSamochodow.App.Domain.Payments;

internal sealed class DebitCardPayment : Payment
{
    public string CardLast4 { get; }
    public DebitCardPayment(Money amount, DateTime paidAt, string cardLast4) : base(amount, paidAt)
    {

    }
    public override string MethodName => "Karta debetowa";
    public override string Describe() => $"Karta **** {CardLast4}";
}
