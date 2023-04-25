using System.Collections.Generic;

namespace HerPublicWebsite.Models.Questionnaire;

public record class SelectAddressViewModel : QuestionFlowViewModel
{
    public List<OsPlacesResult> Addresses {get; set;}
}
