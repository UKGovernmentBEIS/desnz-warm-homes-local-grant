using GovUkDesignSystem.Attributes.ValidationAttributes;
using HerPublicWebsite.BusinessLogic.Models.Enums;

namespace HerPublicWebsite.Models.Questionnaire
{
    public class GasBoilerViewModel : QuestionFlowViewModel
    {
        [GovUkValidateRequired(ErrorMessageIfMissing = "Select yes if you have a gas boiler, or no if you do not have a gas boiler")]
        public HasGasBoiler? HasGasBoiler { get; set; }
    }
}
