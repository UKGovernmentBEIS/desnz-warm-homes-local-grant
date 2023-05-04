using GovUkDesignSystem.Attributes.ValidationAttributes;
using HerPublicWebsite.BusinessLogic.Models.Enums;

namespace HerPublicWebsite.Models.Questionnaire
{
    public class GasBoilerViewModel : QuestionFlowViewModel
    {
        [GovUkValidateRequired(ErrorMessageIfMissing = "Select yes if you have a gas boiler, or I do not know if you're not sure")]
        public HasGasBoiler? HasGasBoiler { get; set; }
    }
}