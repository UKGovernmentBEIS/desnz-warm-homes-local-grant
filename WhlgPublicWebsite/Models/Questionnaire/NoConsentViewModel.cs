using GovUkDesignSystem.Attributes.ValidationAttributes;
using HerPublicWebsite.Models.Enums;

namespace HerPublicWebsite.Models.Questionnaire;

public class NoConsentViewModel : QuestionFlowViewModel
{
    public string LocalAuthorityName { get; set; }
    
    public string LocalAuthorityWebsite { get; set; }
}
