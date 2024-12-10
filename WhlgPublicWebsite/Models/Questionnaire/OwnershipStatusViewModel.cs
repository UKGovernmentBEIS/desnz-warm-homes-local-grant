using GovUkDesignSystem.Attributes.ValidationAttributes;
using WhlgPublicWebsite.BusinessLogic.Models.Enums;

namespace WhlgPublicWebsite.Models.Questionnaire;

public class OwnershipStatusViewModel : QuestionFlowViewModel
{
    [GovUkValidateRequired(ErrorMessageIfMissing = "Select your ownership status of the property")]
    public OwnershipStatus? OwnershipStatus { get; set; }
}
