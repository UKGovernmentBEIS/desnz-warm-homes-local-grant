using GovUkDesignSystem.Attributes.ValidationAttributes;
using SeaPublicWebsite.BusinessLogic.Models.Enums;

namespace SeaPublicWebsite.Models.EnergyEfficiency
{
    public class OutdoorSpaceViewModel : QuestionFlowViewModel
    {
        [GovUkValidateRequired(ErrorMessageIfMissing = "Select whether you have outdoor space")]
        public HasOutdoorSpace? HasOutdoorSpace { get; set; }
        public bool HintHasOutdoorSpace { get; set; }

        public string Reference { get; set; }
    }
}