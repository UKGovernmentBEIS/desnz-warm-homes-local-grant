using GovUkDesignSystem.Attributes.ValidationAttributes;
using HerPublicWebsite.BusinessLogic.Models;
using HerPublicWebsite.BusinessLogic.Models.Enums;

namespace HerPublicWebsite.Models.EnergyEfficiency
{
    public class HotWaterCylinderViewModel : QuestionFlowViewModel
    {
        [GovUkValidateRequired(ErrorMessageIfMissing = "Select whether you have a hot water cylinder")]
        public HasHotWaterCylinder? HasHotWaterCylinder { get; set; }

        public string Reference { get; set; }
        public Epc Epc { get; set; }
    }
}