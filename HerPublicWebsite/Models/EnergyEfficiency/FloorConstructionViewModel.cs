using GovUkDesignSystem.Attributes.ValidationAttributes;
using HerPublicWebsite.BusinessLogic.Models;
using HerPublicWebsite.BusinessLogic.Models.Enums;

namespace HerPublicWebsite.Models.EnergyEfficiency
{
    public class FloorConstructionViewModel : QuestionFlowViewModel
    {
        [GovUkValidateRequired(ErrorMessageIfMissing = "Select how your floor is built")]
        public FloorConstruction? FloorConstruction { get; set; }
        public bool? HintSuspendedTimber { get; set; }

        public string Reference { get; set; }
        public Epc Epc { get; set; }
    }
}