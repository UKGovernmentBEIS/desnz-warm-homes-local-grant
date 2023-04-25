using GovUkDesignSystem.Attributes.ValidationAttributes;
using HerPublicWebsite.BusinessLogic.Models.Enums;

namespace HerPublicWebsite.Models.Questionnaire
{
    public record class CountryViewModel : QuestionFlowViewModel
    {
        [GovUkValidateRequired(ErrorMessageIfMissing = "Select which country the property is located in")]
        public Country? Country { get; set; }
    }
}
