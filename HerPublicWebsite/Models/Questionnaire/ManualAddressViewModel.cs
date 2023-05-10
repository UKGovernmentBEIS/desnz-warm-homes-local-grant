using GovUkDesignSystem.Attributes.ValidationAttributes;

namespace HerPublicWebsite.Models.Questionnaire;

public class ManualAddressViewModel : QuestionFlowViewModel
{
    [GovUkValidateRequired(ErrorMessageIfMissing = "Enter an address")]
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    [GovUkValidateRequired(ErrorMessageIfMissing = "Enter a town or city")]
    public string Town { get; set; }
    public string County { get; set; }
    [GovUkValidateRequired(ErrorMessageIfMissing = "Enter a valid postcode")]
    public string Postcode { get; set; }
}
