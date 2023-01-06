using GovUkDesignSystem.Attributes.ValidationAttributes;
using SeaPublicWebsite.BusinessLogic.Models;
using SeaPublicWebsite.BusinessLogic.Models.Enums;

namespace SeaPublicWebsite.Models.EnergyEfficiency
{
    public class FloorInsulatedViewModel : QuestionFlowViewModel
    {
        [GovUkValidateRequired(ErrorMessageIfMissing = "Select if the floor is insulated")]
        public FloorInsulated? FloorInsulated { get; set; }
        public bool? HintUninsulatedFloor { get; set; }

        public string Reference { get; set; }
        public Epc Epc { get; set; }
    }
}