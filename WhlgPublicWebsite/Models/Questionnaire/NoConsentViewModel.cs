using GovUkDesignSystem.Attributes.ValidationAttributes;
using WhlgPublicWebsite.Models.Enums;

namespace WhlgPublicWebsite.Models.Questionnaire;

public class NoConsentViewModel : QuestionFlowViewModel
{
    public string LocalAuthorityName { get; set; }
    
    public string LocalAuthorityWebsite { get; set; }
}
