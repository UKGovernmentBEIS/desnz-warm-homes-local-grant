using GovUkDesignSystem.Attributes.ValidationAttributes;
using WhlgPublicWebsite.Models.Enums;

namespace WhlgPublicWebsite.Models.Questionnaire;

public class ConfirmLocalAuthorityViewModel : QuestionFlowViewModel
{
    public string LocalAuthorityName { get; set; }
    [GovUkValidateRequired(ErrorMessageIfMissing = "Select whether this is your Local Authority")]
    public YesOrNo? LaIsCorrect { get; set; }
}
