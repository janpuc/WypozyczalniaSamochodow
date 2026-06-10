using WypozyczalniaSamochodow.App.Domain.Shared;

namespace WypozyczalniaSamochodow.App.Domain.Payments;

internal abstract class Payment
{
    public Money Amount { get; }
    public DateTime PaidAt { get; }

    protected Payment(Money amount, DateTime paidAt)
    {
        Amount = amount;
        PaidAt = paidAt;
    }

    public abstract string MethodName { get; }
    public abstract string Describe();
}
