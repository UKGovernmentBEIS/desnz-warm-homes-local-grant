using GovUkDesignSystem.Attributes.ValidationAttributes;

namespace WhlgPublicWebsite.Models.Auth;

public class AuthViewModel
{
    [GovUkValidateRequired(ErrorMessageIfMissing = "Enter password")]
    public string Password { get; set; }

    public string ReturnPath { get; set; }
}