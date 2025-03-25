using GovUkDesignSystem.Attributes.ValidationAttributes;
using WhlgPublicWebsite.BusinessLogic.Models.Enums;

namespace WhlgPublicWebsite.Models.Questionnaire
{
    public class CountryViewModel : QuestionFlowViewModel
    {
        [GovUkValidateRequired(ErrorMessageIfMissing = "Select which country the property is located in")]
        public Country? Country { get; set; }
    }
}
