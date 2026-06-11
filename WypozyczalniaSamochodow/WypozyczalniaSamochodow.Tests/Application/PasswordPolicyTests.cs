using AwesomeAssertions;

using WypozyczalniaSamochodow.App.Application.Auth;

using Xunit;

namespace WypozyczalniaSamochodow.Tests.Application;

public sealed class PasswordPolicyTests
{
    [Theory]
    [InlineData(null, false)]
    [InlineData("       ", false)]
    [InlineData("short", false)]
    [InlineData("exactly8", true)]
    [InlineData("longer-password", true)]
    public void IsSatisfiedByEnforcesNonBlankMinimumLength(string? password, bool expected)
    {
        PasswordPolicy.IsSatisfiedBy(password).Should().Be(expected);
    }
}
