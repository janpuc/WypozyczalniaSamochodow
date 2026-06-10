using WypozyczalniaSamochodow.App.Domain.Users;

namespace WypozyczalniaSamochodow.App.Presentation.Abstraction;

internal interface IPrompts
{
    string PromptText(string label, string? defaultValue = null, bool allowEmpty = false);
    string PromptSecret(string label, string? defaultValue = null);
    bool PromptConfirm(string label);
    string PromptChoice(string label, IEnumerable<string> choices);
    string PromptFullName(string? defaultValue = null);
    string PromptEmail(string? defaultValue = null);
    string PromptPassword(int minLength = 0);
    string PromptPasswordConfirmation(string password);
    DrivingLicence PromptDrivingLicence(DrivingLicence? existing = null);
    DateOnly PromptDate(string label, DateOnly? defaultValue = null, DateOnly? notBefore = null);
    int PromptInt(string label);
    decimal PromptDecimal(string label);
}
