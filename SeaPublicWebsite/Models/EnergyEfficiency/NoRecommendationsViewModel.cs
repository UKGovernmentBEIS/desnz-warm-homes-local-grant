using GovUkDesignSystem.Attributes.ValidationAttributes;
using SeaPublicWebsite.BusinessLogic.Models.Enums;

namespace SeaPublicWebsite.Models.EnergyEfficiency;

public class NoRecommendationsViewModel: QuestionFlowViewModel
{
    public string Reference { get; set; }
    [GovUkValidateRequired(ErrorMessageIfMissing = "Enter a valid email address")]
    public string EmailAddress { get; set; }
    public bool EmailSent { get; set; }
    
    public HasOutdoorSpace? HasOutdoorSpace { get; set; }
}