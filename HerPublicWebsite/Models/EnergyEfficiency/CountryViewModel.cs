using GovUkDesignSystem.Attributes.ValidationAttributes;
using HerPublicWebsite.BusinessLogic.Models.Enums;

namespace HerPublicWebsite.Models.EnergyEfficiency
{
    public class CountryViewModel : QuestionFlowViewModel
    {
        [GovUkValidateRequired(ErrorMessageIfMissing = "Select which country the property is located in")]
        public Country? Country { get; set; }
        
        public string Reference { get; set; }
    }
}
