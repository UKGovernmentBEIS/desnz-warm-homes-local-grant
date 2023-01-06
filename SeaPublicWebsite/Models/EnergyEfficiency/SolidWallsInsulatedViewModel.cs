using GovUkDesignSystem.Attributes.ValidationAttributes;
using SeaPublicWebsite.BusinessLogic.Models;
using SeaPublicWebsite.BusinessLogic.Models.Enums;

namespace SeaPublicWebsite.Models.EnergyEfficiency
{
    public class SolidWallsInsulatedViewModel : QuestionFlowViewModel
    {
        [GovUkValidateRequired(ErrorMessageIfMissing = "Select if your solid walls are insulated")]
        public SolidWallsInsulated? SolidWallsInsulated { get; set; }

        public string Reference { get; set; }
        public Epc Epc { get; set; }
    }
}