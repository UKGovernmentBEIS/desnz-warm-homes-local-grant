using GovUkDesignSystem.Attributes.ValidationAttributes;
using HerPublicWebsite.BusinessLogic.Models;
using HerPublicWebsite.BusinessLogic.Models.Enums;

namespace HerPublicWebsite.Models.EnergyEfficiency
{
    public class OtherHeatingTypeViewModel : QuestionFlowViewModel
    {
        [GovUkValidateRequired(ErrorMessageIfMissing = "Select your main heating system")]
        public OtherHeatingType? OtherHeatingType { get; set; }

        public string Reference { get; set; }
        public Epc Epc { get; set; }
    }
}