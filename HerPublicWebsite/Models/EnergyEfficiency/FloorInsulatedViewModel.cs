using GovUkDesignSystem.Attributes.ValidationAttributes;
using HerPublicWebsite.BusinessLogic.Models;
using HerPublicWebsite.BusinessLogic.Models.Enums;

namespace HerPublicWebsite.Models.EnergyEfficiency
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