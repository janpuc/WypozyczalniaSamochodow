using Spectre.Console;

using WypozyczalniaSamochodow.App.Domain.Users;
using WypozyczalniaSamochodow.App.Presentation.Abstraction;
using WypozyczalniaSamochodow.App.Presentation.UIConfig;

namespace WypozyczalniaSamochodow.App.Presentation;

internal sealed class Prompts : IPrompts
{
    private static string Label(string text) => throw new NotImplementedException();

    private static string DateLabel(string text) => throw new NotImplementedException();

    public string PromptText(string label, string? defaultValue = null, bool allowEmpty = false)
    {
        throw new NotImplementedException();
    }

    public string PromptSecret(string label, string? defaultValue = null)
    {
        throw new NotImplementedException();
    }

    public bool PromptConfirm(string label) => throw new NotImplementedException();

    public string PromptChoice(string label, IEnumerable<string> choices) => throw new NotImplementedException();

    public string PromptFullName(string? defaultValue = null) => throw new NotImplementedException();

    public string PromptEmail(string? defaultValue = null) => throw new NotImplementedException();

    public string PromptPassword(int minLength = 0) => throw new NotImplementedException();

    public string PromptPasswordConfirmation(string password) => throw new NotImplementedException();

    public DrivingLicence PromptDrivingLicence(DrivingLicence? existing = null)
    {
        throw new NotImplementedException();
    }

    public DateOnly PromptDate(string label, DateOnly? defaultValue = null, DateOnly? notBefore = null)
    {
        throw new NotImplementedException();
    }

    public int PromptInt(string label) => throw new NotImplementedException();

    public decimal PromptDecimal(string label) => throw new NotImplementedException();
}
