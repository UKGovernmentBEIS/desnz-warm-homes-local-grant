using GovUkDesignSystem.Attributes.ValidationAttributes;
using HerPublicWebsite.BusinessLogic.Models;
using HerPublicWebsite.BusinessLogic.Models.Enums;

namespace HerPublicWebsite.Models.EnergyEfficiency
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