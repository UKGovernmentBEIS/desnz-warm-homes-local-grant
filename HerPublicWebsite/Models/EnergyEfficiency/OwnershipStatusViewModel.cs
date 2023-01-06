using GovUkDesignSystem.Attributes.ValidationAttributes;
using HerPublicWebsite.BusinessLogic.Models.Enums;

namespace HerPublicWebsite.Models.EnergyEfficiency
{
    public class OwnershipStatusViewModel : QuestionFlowViewModel
    {
        public string Reference { get; set; }

        [GovUkValidateRequired(ErrorMessageIfMissing = "Select your circumstances")]
        public OwnershipStatus? OwnershipStatus { get; set; }
    }
}