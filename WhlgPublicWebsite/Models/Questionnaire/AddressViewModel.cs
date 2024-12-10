using GovUkDesignSystem.Attributes.ValidationAttributes;

namespace WhlgPublicWebsite.Models.Questionnaire;

public class AddressViewModel : QuestionFlowViewModel
{
    public string BuildingNameOrNumber { get; set; }

    [GovUkValidateRequired(ErrorMessageIfMissing = "Enter a postcode")]
    public string Postcode { get; set; }
}
