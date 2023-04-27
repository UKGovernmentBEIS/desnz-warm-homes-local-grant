using System.Collections.Generic;
using GovUkDesignSystem.Attributes.ValidationAttributes;

namespace HerPublicWebsite.Models.Questionnaire;

public record class SelectAddressViewModel : QuestionFlowViewModel
{
    public List<OsPlacesResult> Addresses {get; set;}
    [GovUkValidateRequired(ErrorMessageIfMissing = "You must select an address")]
    public string Index {get; set;}
}
