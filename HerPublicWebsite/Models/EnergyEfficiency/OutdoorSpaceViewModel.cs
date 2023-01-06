using GovUkDesignSystem.Attributes.ValidationAttributes;
using HerPublicWebsite.BusinessLogic.Models.Enums;

namespace HerPublicWebsite.Models.EnergyEfficiency
{
    public class OutdoorSpaceViewModel : QuestionFlowViewModel
    {
        [GovUkValidateRequired(ErrorMessageIfMissing = "Select whether you have outdoor space")]
        public HasOutdoorSpace? HasOutdoorSpace { get; set; }
        public bool HintHasOutdoorSpace { get; set; }

        public string Reference { get; set; }
    }
}