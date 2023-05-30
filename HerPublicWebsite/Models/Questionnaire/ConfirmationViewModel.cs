using GovUkDesignSystem.Attributes.ValidationAttributes;
using HerPublicWebsite.Models.Enums;

namespace HerPublicWebsite.Models.Questionnaire;

public class ConfirmationViewModel : QuestionFlowViewModel
{
    public string ReferenceCode { get; set; }
    public string LocalAuthorityName { get; set; }
    public string LocalAuthorityWebsite { get; set; }
    public string ConfirmationEmailAddress { get; set; }
    [GovUkValidateRequired(ErrorMessageIfMissing = "Select whether we can notify you about future energy grants")]
    public YesOrNo? CanNotfiyAboutFutureSchemes { get; set; }
}
