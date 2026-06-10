using Xunit;
using AwesomeAssertions;
using NSubstitute;
using WypozyczalniaSamochodow.App.Domain.Users;
using WypozyczalniaSamochodow.Tests.TestSupport;
using WypozyczalniaSamochodow.App.Domain.Shared;

namespace WypozyczalniaSamochodow.Tests.Domain;

public sealed class UsersTests
{
    [Fact]
    public void EmailNormalizesAndComparesByValue()
    {
        var email = new Email("  Test@Example.COM  ");

        email.Value.Should().Be("test@example.com");
        email.ToString().Should().Be("test@example.com");
        email.Equals(new Email("test@example.com")).Should().BeTrue();
        email.Should().Be(new Email(" TEST@example.com "));
        email.GetHashCode().Should().Be(new Email("test@example.com").GetHashCode());
    }

    [Fact]
    public void EmailRejectsInvalidValue()
    {
        var act = () => new Email("bad-email");

        act.Should().Throw<DomainException>()
            .WithMessage("Podaj prawidłowy adres email.");
    }

    [Theory]
    [InlineData("  Test@Example.COM  ", true, "test@example.com")]
    [InlineData("bad-email", false, null)]
    [InlineData("", false, null)]
    public void EmailTryCreateValidatesWithoutThrowing(string input, bool expectedOk, string? expectedValue)
    {
        var ok = Email.TryCreate(input, out var email);

        ok.Should().Be(expectedOk);
        if (expectedOk)
            email.Value.Should().Be(expectedValue);
    }

    [Fact]
    public void EmailTryCreateReturnsFalseForNullInput()
    {
        Email.TryCreate(null!, out var email).Should().BeFalse();
        email.Should().BeNull();
    }

    [Fact]
    public void PasswordUsesHasherAndVerifies()
    {
        var hasher = Substitute.For<IPasswordHasher>();
        hasher.Hash("secret").Returns("hash");
        hasher.Verify("secret", "hash").Returns(true);

        var password = Password.FromPlain("secret", hasher);

        password.Hash.Should().Be("hash");
        password.Verify("secret", hasher).Should().BeTrue();
        hasher.Received(1).Hash("secret");
    }

    [Fact]
    public void PasswordRejectsEmptyValues()
    {
        var hasher = Substitute.For<IPasswordHasher>();

        Action act1 = () => Password.FromPlain("", hasher);
        Action act2 = () => Password.FromHash(" ");

        act1.Should().Throw<DomainException>();
        act2.Should().Throw<DomainException>();
    }

    [Fact]
    public void DrivingLicenceNormalizesDateAndValidatesRange()
    {
        var licence = new DrivingLicence("ABC123", new DateOnly(2026, 5, 25));

        licence.ExpiryDate.Should().Be(new DateOnly(2026, 5, 25));
        licence.IsValidOn(new DateOnly(2026, 5, 25)).Should().BeTrue();
        licence.IsValidOn(new DateOnly(2026, 5, 26)).Should().BeFalse();
    }

    [Fact]
    public void DrivingLicenceRejectsEmptyNumber()
    {
        var act = () => new DrivingLicence(" ", new DateOnly(2026, 5, 25));

        act.Should().Throw<DomainException>()
            .WithMessage("Numer prawa jazdy nie może być pusty.");
    }

    [Fact]
    public void UserAndClientMutatorsWork()
    {
        var hasher = Substitute.For<IPasswordHasher>();
        hasher.Hash(Arg.Any<string>()).Returns(call => $"hash:{call.Arg<string>()}");
        var client = new Client("Jan Kowalski", new Email("jan@example.com"), Password.FromPlain("secret", hasher));
        var backoffice = new Backoffice("Admin", new Email("admin@example.com"), Password.FromHash("stored"));

        client.Rename("Jan Nowak");
        client.ChangeEmail(new Email("jan.nowak@example.com"));
        client.ResetPassword("new-secret", hasher);

        client.FullName.Should().Be("Jan Nowak");
        client.Email.Should().Be(new Email("jan.nowak@example.com"));
        client.Password.Hash.Should().Be("hash:new-secret");
        backoffice.FullName.Should().Be("Admin");

        client.RemoveLicence();
        client.DrivingLicence.Should().BeNull();
    }

    [Fact]
    public void UserRejectsEmptyNamesInConstructorAndRename()
    {
        Action act1 = () => new Backoffice(" ", new Email("admin@example.com"), Password.FromHash("stored"));
        var user = new Backoffice("Admin", new Email("admin@example.com"), Password.FromHash("stored"));
        Action act2 = () => user.Rename(" ");

        act1.Should().Throw<DomainException>()
            .WithMessage("Imię i nazwisko nie może być puste.");
        act2.Should().Throw<DomainException>()
            .WithMessage("Imię i nazwisko nie może być puste.");
    }

    [Fact]
    public void ClientEnsureCanRentRequiresValidLicence()
    {
        var hasher = Substitute.For<IPasswordHasher>();
        hasher.Hash("secret").Returns("hash");
        var clock = new FixedClock(new DateOnly(2026, 5, 25));

        var withoutLicence = new Client("Jan", new Email("jan@example.com"), Password.FromPlain("secret", hasher));
        var expiredLicenceClient = new Client("Jan", new Email("jan2@example.com"), Password.FromPlain("secret", hasher), new DrivingLicence("ABC", new DateOnly(2026, 5, 24)));
        var okClient = new Client("Jan", new Email("jan3@example.com"), Password.FromPlain("secret", hasher), new DrivingLicence("ABC", new DateOnly(2026, 5, 26)));

        Action act3 = () => withoutLicence.EnsureCanRent(clock);
        Action act4 = () => expiredLicenceClient.EnsureCanRent(clock);
        Action act5 = () => okClient.EnsureCanRent(clock);

        act3.Should().Throw<DomainException>().WithMessage("Klient nie posiada prawa jazdy.");
        act4.Should().Throw<DomainException>().WithMessage("Prawo jazdy klienta jest nieważne.");
        act5.Should().NotThrow();
    }
}

