using GovUkDesignSystem.Attributes.ValidationAttributes;
using HerPublicWebsite.BusinessLogic.Models.Enums;

namespace HerPublicWebsite.Models.EnergyEfficiency
{
    public class LoftSpaceViewModel : QuestionFlowViewModel
    {
        [GovUkValidateRequired(ErrorMessageIfMissing = "Select if you have a loft space")]
        public LoftSpace? LoftSpace { get; set; }
        public bool HintHaveLoftAndAccess { get; set; }
        public string Reference { get; set; }
    }
}