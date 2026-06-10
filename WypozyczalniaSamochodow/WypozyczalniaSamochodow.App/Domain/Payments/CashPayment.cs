using WypozyczalniaSamochodow.App.Domain.Shared;

namespace WypozyczalniaSamochodow.App.Domain.Payments;

internal sealed class CashPayment : Payment
{
    public CashPayment(Money amount, DateTime paidAt) : base(amount, paidAt) { }
    public override string MethodName => "Gotówka";
    public override string Describe() => "Gotówka";
}
