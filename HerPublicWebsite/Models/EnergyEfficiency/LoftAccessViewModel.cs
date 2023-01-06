using GovUkDesignSystem.Attributes.ValidationAttributes;
using HerPublicWebsite.BusinessLogic.Models.Enums;

namespace HerPublicWebsite.Models.EnergyEfficiency
{
    public class LoftAccessViewModel : QuestionFlowViewModel
    {
        [GovUkValidateRequired(ErrorMessageIfMissing = "Select if you have loft access")]
        public LoftAccess? LoftAccess { get; set; }
        public bool HintHaveLoftAndAccess { get; set; }

        public string Reference { get; set; }
    }
}