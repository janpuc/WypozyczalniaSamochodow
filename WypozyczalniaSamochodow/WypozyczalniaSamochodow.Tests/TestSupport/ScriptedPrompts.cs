using WypozyczalniaSamochodow.App.Domain.Users;
using WypozyczalniaSamochodow.App.Presentation.Abstraction;

namespace WypozyczalniaSamochodow.Tests.TestSupport;

internal sealed class ScriptedPrompts : IPrompts
{
    private readonly Queue<string> _texts = new();
    private readonly Queue<string> _secrets = new();
    private readonly Queue<bool> _confirms = new();
    private readonly Queue<string> _choices = new();
    private readonly Queue<DrivingLicence> _licences = new();
    private readonly Queue<DateOnly> _dates = new();
    private readonly Queue<int> _ints = new();
    private readonly Queue<decimal> _decimals = new();

    public ScriptedPrompts EnqueueText(params string[] values) { foreach (var value in values) _texts.Enqueue(value); return this; }
    public ScriptedPrompts EnqueueSecret(params string[] values) { foreach (var value in values) _secrets.Enqueue(value); return this; }
    public ScriptedPrompts EnqueueConfirm(params bool[] values) { foreach (var value in values) _confirms.Enqueue(value); return this; }
    public ScriptedPrompts EnqueueChoice(params string[] values) { foreach (var value in values) _choices.Enqueue(value); return this; }
    public ScriptedPrompts EnqueueLicence(params DrivingLicence[] values) { foreach (var value in values) _licences.Enqueue(value); return this; }
    public ScriptedPrompts EnqueueDate(params DateOnly[] values) { foreach (var value in values) _dates.Enqueue(value); return this; }
    public ScriptedPrompts EnqueueInt(params int[] values) { foreach (var value in values) _ints.Enqueue(value); return this; }
    public ScriptedPrompts EnqueueDecimal(params decimal[] values) { foreach (var value in values) _decimals.Enqueue(value); return this; }

    public string PromptText(string label, string? defaultValue = null, bool allowEmpty = false) => _texts.Dequeue();
    public string PromptSecret(string label, string? defaultValue = null) => _secrets.Dequeue();
    public bool PromptConfirm(string label) => _confirms.Dequeue();
    public string PromptChoice(string label, IEnumerable<string> choices) => _choices.Dequeue();
    public string PromptFullName(string? defaultValue = null) => _texts.Dequeue();
    public string PromptEmail(string? defaultValue = null) => _texts.Dequeue();
    public string PromptPassword(int minLength = 0) => _secrets.Count > 0 ? _secrets.Dequeue() : _texts.Dequeue();
    public string PromptPasswordConfirmation(string password) => _secrets.Count > 0 ? _secrets.Dequeue() : _texts.Dequeue();
    public DrivingLicence PromptDrivingLicence(DrivingLicence? existing = null) => _licences.Dequeue();
    public DateOnly PromptDate(string label, DateOnly? defaultValue = null, DateOnly? notBefore = null) => _dates.Dequeue();
    public int PromptInt(string label) => _ints.Dequeue();
    public decimal PromptDecimal(string label) => _decimals.Dequeue();
}
