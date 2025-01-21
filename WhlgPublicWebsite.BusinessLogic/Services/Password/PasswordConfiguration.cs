namespace WhlgPublicWebsite.BusinessLogic.Services.Password;

public class PasswordConfiguration
{
    public const string ConfigSection = "Auth";

    // We provide default passwords in the DEV and Staging appsettings files to ensure a password is always set.
    // Though, we expect this password to be overriden at the infrastructure layer
    public string Password { get; set; }
}