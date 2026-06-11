using Spectre.Console;

using WypozyczalniaSamochodow.App.Domain.Shared;
using WypozyczalniaSamochodow.App.Domain.Users;
using WypozyczalniaSamochodow.App.Presentation.Abstraction;
using WypozyczalniaSamochodow.App.Presentation.UIConfig;

namespace WypozyczalniaSamochodow.App.Presentation;

internal sealed class Prompts : IPrompts
{
    private static string Label(string text) =>
        ConsoleMarkup.Wrap($"{text}{UiStrings.PromptLabelSuffix}", Theme.MarkupOf(UiRole.Prompt));

    private static string DateLabel(string text) => Label($"{text}{UiStrings.PromptDateHint}");

    public string PromptText(string label, string? defaultValue = null, bool allowEmpty = false)
    {
        var prompt = defaultValue is null
            ? new TextPrompt<string>(Label(label))
            : new TextPrompt<string>(Label(label)).DefaultValue(defaultValue);

        if (allowEmpty)
            prompt = prompt.AllowEmpty();

        return AnsiConsole.Prompt(prompt);
    }

    public string PromptSecret(string label, string? defaultValue = null)
    {
        var prompt = defaultValue is null
            ? new TextPrompt<string>(Label(label))
            : new TextPrompt<string>(Label(label)).DefaultValue(defaultValue);

        return AnsiConsole.Prompt(prompt.Secret());
    }

    public bool PromptConfirm(string label) => AnsiConsole.Confirm(ConsoleMarkup.Wrap(label, Theme.MarkupOf(UiRole.Prompt)));

    public string PromptChoice(string label, IEnumerable<string> choices) =>
        AnsiConsole.Prompt(new SelectionPrompt<string>()
            .Title(Label(label))
            .AddChoices(choices));

    public string PromptFullName(string? defaultValue = null) =>
        PromptText(UiStrings.FullName, defaultValue);

    public string PromptEmail(string? defaultValue = null) =>
        AnsiConsole.Prompt((defaultValue is null
                ? new TextPrompt<string>(Label(UiStrings.Email))
                : new TextPrompt<string>(Label(UiStrings.Email)).DefaultValue(defaultValue))
            .Validate(s =>
            {
                try { _ = new Email(s); return ValidationResult.Success(); }
                catch (DomainException ex) { return ValidationResult.Error(ex.Message); }
            }));

    public string PromptPassword(int minLength = 0) =>
        AnsiConsole.Prompt(new TextPrompt<string>(Label(UiStrings.PromptPassword))
            .Secret()
            .Validate(s =>
            {
                if (string.IsNullOrWhiteSpace(s)) return ValidationResult.Error(UiStrings.ValidationPasswordEmpty);
                if (s.Length < minLength) return ValidationResult.Error(string.Format(UiStrings.ValidationPasswordTooShort, minLength));
                return ValidationResult.Success();
            }));

    public string PromptPasswordConfirmation(string password) =>
        AnsiConsole.Prompt(new TextPrompt<string>(Label(UiStrings.PromptPasswordConfirm))
            .Secret()
            .Validate(s => s == password ? ValidationResult.Success() : ValidationResult.Error(UiStrings.ValidationPasswordMismatch)));

    public DrivingLicence PromptDrivingLicence(DrivingLicence? existing = null)
    {
        var number = AnsiConsole.Prompt(existing is null
            ? new TextPrompt<string>(Label(UiStrings.LicenceNumber))
            : new TextPrompt<string>(Label(UiStrings.LicenceNumber)).DefaultValue(existing.Number));
        var expiry = AnsiConsole.Prompt(existing is null
            ? new TextPrompt<DateOnly>(DateLabel(UiStrings.LicenceExpiry))
            : new TextPrompt<DateOnly>(DateLabel(UiStrings.LicenceExpiry)).DefaultValue(existing.ExpiryDate));
        return new DrivingLicence(number, expiry);
    }

    public DateOnly PromptDate(string label, DateOnly? defaultValue = null, DateOnly? notBefore = null)
    {
        var p = defaultValue.HasValue
            ? new TextPrompt<DateOnly>(DateLabel(label)).DefaultValue(defaultValue.Value)
            : new TextPrompt<DateOnly>(DateLabel(label));
        if (notBefore.HasValue)
            p = p.Validate(d => d < notBefore.Value
                ? ValidationResult.Error(string.Format(UiStrings.ValidationDateNotBefore, notBefore.Value.ToString(UiFormats.Date)))
                : ValidationResult.Success());
        return AnsiConsole.Prompt(p);
    }

    public int PromptInt(string label) => AnsiConsole.Prompt(new TextPrompt<int>(Label(label)));

    public decimal PromptDecimal(string label) => AnsiConsole.Prompt(new TextPrompt<decimal>(Label(label)));
}
