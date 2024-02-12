using GovUkDesignSystem.Attributes.ValidationAttributes;
using HerPublicWebsite.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace HerPublicWebsite.Models.Questionnaire;

public class PendingViewModel : QuestionFlowViewModel
{
    public string LocalAuthorityName { get; set; }
    
    public string LocalAuthorityMessagePartialViewPath { get; set; }
    
    [GovUkValidateRequired(ErrorMessageIfMissing = "You must acknowledge that your application will not be processed until your local authority has signed up to use the service")]
    public bool UserAcknowledgesApplicationNotProcessedUntilLocalAuthorityLive { get; set; }

    public bool Submitted { get; set; }
}