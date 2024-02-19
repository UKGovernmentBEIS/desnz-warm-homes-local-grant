using GovUkDesignSystem.Attributes.ValidationAttributes;
using HerPublicWebsite.Models.Enums;
using System.ComponentModel.DataAnnotations;
using GovUkDesignSystem.Attributes;
using GovUkDesignSystem.ModelBinders;
using Microsoft.AspNetCore.Mvc;

namespace HerPublicWebsite.Models.Questionnaire;



public class PendingViewModel : QuestionFlowViewModel
{
    public string LocalAuthorityName { get; set; }
    
    public string LocalAuthorityMessagePartialViewPath { get; set; }
    
    [ModelBinder(typeof(GovUkCheckboxBoolBinder))]
    [Range(typeof(bool), "true", "true", ErrorMessage="You must acknowledge that your application will not be processed until your local authority has signed up to use the service")]
    public bool UserAcknowledgesApplicationNotProcessedUntilLocalAuthorityLive { get; set; }

    public bool Submitted { get; set; }
}