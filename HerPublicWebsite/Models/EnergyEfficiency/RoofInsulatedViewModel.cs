using GovUkDesignSystem.Attributes.ValidationAttributes;
using HerPublicWebsite.BusinessLogic.Models;
using HerPublicWebsite.BusinessLogic.Models.Enums;

namespace HerPublicWebsite.Models.EnergyEfficiency
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