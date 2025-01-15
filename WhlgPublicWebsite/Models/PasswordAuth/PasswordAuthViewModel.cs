using GovUkDesignSystem.Attributes.ValidationAttributes;

namespace WhlgPublicWebsite.Models.PasswordAuth;

public class PasswordAuthViewModel
{
    [GovUkValidateRequired(ErrorMessageIfMissing = "Enter password")]
    public string Password { get; set; }
    
    public string ReturnPath { get; set; }
}