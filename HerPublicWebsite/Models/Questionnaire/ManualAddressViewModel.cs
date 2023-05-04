using GovUkDesignSystem.Attributes.ValidationAttributes;

namespace HerPublicWebsite.Models.Questionnaire;

public class ManualAddressViewModel: QuestionFlowViewModel
{
    [GovUkValidateRequired(ErrorMessageIfMissing = "You must provide an address")]
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    [GovUkValidateRequired(ErrorMessageIfMissing = "You must provide a town")]
    public string Town { get; set; }
    public string County { get; set; }
    [GovUkValidateRequired(ErrorMessageIfMissing = "You must provide a valid postcode")]
    public string Postcode { get; set; }
}