using WypozyczalniaSamochodow.App.Domain.Shared;

namespace WypozyczalniaSamochodow.App.Domain.Payments;

internal sealed class BitcoinPayment : Payment
{
    public string WalletAddress { get; }
    public BitcoinPayment(Money amount, DateTime paidAt, string walletAddress) : base(amount, paidAt)
    {
        if (string.IsNullOrWhiteSpace(walletAddress))
            throw new DomainException("Adres portfela nie może być pusty.");
        WalletAddress = walletAddress.Trim();
    }
    public override string MethodName => "Bitcoin";
    public override string Describe() => $"Bitcoin ({WalletAddress})";
}
