using GovUkDesignSystem.Attributes.ValidationAttributes;
using HerPublicWebsite.BusinessLogic.Models.Enums;

namespace HerPublicWebsite.Models.Questionnaire
{
    public class GasBoilerViewModel : QuestionFlowViewModel
    {
        [GovUkValidateRequired(ErrorMessageIfMissing = "Select yes if you have a mains gas boiler, or no if you do not have a mains gas boiler")]
        public HasGasBoiler? HasGasBoiler { get; set; }
    }
}
