using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GovUkDesignSystem.ModelBinders;
using Microsoft.AspNetCore.Mvc;

namespace HerPublicWebsite.Models.Questionnaire;

public class PendingViewModel : QuestionFlowViewModel, IValidatableObject
{
    public string LocalAuthorityName { get; set; }
    
    [ModelBinder(typeof(GovUkCheckboxBoolBinder))]
    public bool UserAcknowledgedPending { get; set; }
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!UserAcknowledgedPending)
        {
            yield return new ValidationResult(
                "You must acknowledge that your application will not be processed until your local authority has signed up to use the service",
                new[] { nameof(UserAcknowledgedPending) });
        }
    }
}