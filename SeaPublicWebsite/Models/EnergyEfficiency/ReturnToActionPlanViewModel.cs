using GovUkDesignSystem.Attributes.ValidationAttributes;

namespace SeaPublicWebsite.Models.EnergyEfficiency;

public class ReturnToActionPlanViewModel
{
    public string Reference { get; set; }
    [GovUkValidateRequired(ErrorMessageIfMissing = "Enter a valid email address")]
    public string EmailAddress { get; set; }
    public bool EmailSent { get; set; }
}