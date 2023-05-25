using GovUkDesignSystem.Attributes.ValidationAttributes;
using HerPublicWebsite.Models.Enums;

namespace HerPublicWebsite.Models.Questionnaire;

public class ConfirmLocalAuthorityViewModel : QuestionFlowViewModel
{
    public string LocalAuthorityName { get; set; }
    [GovUkValidateRequired(ErrorMessageIfMissing = "Select whether this is your local authority")]
    public YesOrNo? LaIsCorrect { get; set; }
}
