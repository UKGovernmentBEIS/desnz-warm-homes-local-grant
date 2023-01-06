using GovUkDesignSystem.Attributes.ValidationAttributes;

namespace SeaPublicWebsite.Models.EnergyEfficiency
{
    public class AskForPostcodeViewModel : QuestionFlowViewModel
    {
        [GovUkValidateRequired(ErrorMessageIfMissing = "Enter a building number or name")]
        public string HouseNameOrNumber { get; set; }
        [GovUkValidateRequired(ErrorMessageIfMissing = "Enter a postcode")]
        public string Postcode { get; set; }
        public string Reference { get; set; }
    }
}