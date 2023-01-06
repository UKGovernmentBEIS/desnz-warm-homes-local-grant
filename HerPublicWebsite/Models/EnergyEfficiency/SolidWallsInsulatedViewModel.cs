using GovUkDesignSystem.Attributes.ValidationAttributes;
using HerPublicWebsite.BusinessLogic.Models;
using HerPublicWebsite.BusinessLogic.Models.Enums;

namespace HerPublicWebsite.Models.EnergyEfficiency
{
    public class SolidWallsInsulatedViewModel : QuestionFlowViewModel
    {
        [GovUkValidateRequired(ErrorMessageIfMissing = "Select if your solid walls are insulated")]
        public SolidWallsInsulated? SolidWallsInsulated { get; set; }

        public string Reference { get; set; }
        public Epc Epc { get; set; }
    }
}