using GovUkDesignSystem.Attributes.ValidationAttributes;
using SeaPublicWebsite.BusinessLogic.Models;
using SeaPublicWebsite.BusinessLogic.Models.Enums;

namespace SeaPublicWebsite.Models.EnergyEfficiency
{
    public class RoofInsulatedViewModel : QuestionFlowViewModel
    {
        [GovUkValidateRequired(ErrorMessageIfMissing = "Select if the roof is insulated")]
        public RoofInsulated? RoofInsulated { get; set; }
        public bool? HintUninsulatedRoof { get; set; }

        public string Reference { get; set; }
        public Epc Epc { get; set; }
    }
}