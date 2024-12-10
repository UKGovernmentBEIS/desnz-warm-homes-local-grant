using GovUkDesignSystem.Attributes.ValidationAttributes;
using WhlgPublicWebsite.Models.Enums;

namespace WhlgPublicWebsite.Models.Questionnaire;

public class ConfirmLocalAuthorityViewModel : QuestionFlowViewModel
{
    public string LocalAuthorityName { get; set; }
    [GovUkValidateRequired(ErrorMessageIfMissing = "Select whether this is your local authority")]
    public YesOrNo? LaIsCorrect { get; set; }
}
