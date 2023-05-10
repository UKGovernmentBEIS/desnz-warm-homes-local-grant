using GovUkDesignSystem.Attributes.ValidationAttributes;

namespace HerPublicWebsite.Models.Questionnaire;

public class AddressViewModel : QuestionFlowViewModel
{
    [GovUkValidateRequired(ErrorMessageIfMissing = "Enter a building number or name")]
    public string BuildingNameOrNumber { get; set; }

    [GovUkValidateRequired(ErrorMessageIfMissing = "Enter a postcode")]
    public string Postcode { get; set; }
}
