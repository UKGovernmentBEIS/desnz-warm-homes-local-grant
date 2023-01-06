using GovUkDesignSystem.Attributes.ValidationAttributes;
using HerPublicWebsite.BusinessLogic.Models.Enums;

namespace HerPublicWebsite.Models.EnergyEfficiency;

public class NoRecommendationsViewModel: QuestionFlowViewModel
{
    public string Reference { get; set; }
    [GovUkValidateRequired(ErrorMessageIfMissing = "Enter a valid email address")]
    public string EmailAddress { get; set; }
    public bool EmailSent { get; set; }
    
    public HasOutdoorSpace? HasOutdoorSpace { get; set; }
}