using GovUkDesignSystem.Attributes.ValidationAttributes;
using SeaPublicWebsite.BusinessLogic.Models;
using SeaPublicWebsite.BusinessLogic.Models.Enums;

namespace SeaPublicWebsite.Models.EnergyEfficiency
{
    public class GlazingTypeViewModel : QuestionFlowViewModel
    {
        [GovUkValidateRequired(ErrorMessageIfMissing = "Select glazing type")]
        public GlazingType? GlazingType { get; set; }
        public bool? HintSingleGlazing { get; set; }

        public string Reference { get; set; }
        public Epc Epc { get; set; }
    }
}