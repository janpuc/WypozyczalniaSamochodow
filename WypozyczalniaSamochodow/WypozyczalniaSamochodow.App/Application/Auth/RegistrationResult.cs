namespace WypozyczalniaSamochodow.App.Application.Auth;

internal enum RegistrationResult
{
    Success,
    InvalidEmail,
    WeakPassword,
    EmailTaken
}
