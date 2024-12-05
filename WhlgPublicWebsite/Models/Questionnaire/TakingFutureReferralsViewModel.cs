using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GovUkDesignSystem.ModelBinders;
using Microsoft.AspNetCore.Mvc;

namespace HerPublicWebsite.Models.Questionnaire;

public class TakingFutureReferralsViewModel : QuestionFlowViewModel, IValidatableObject
{
    public string LocalAuthorityName { get; set; }

    [ModelBinder(typeof(GovUkCheckboxBoolBinder))]
    public bool UserAcknowledgedFutureReferral { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!UserAcknowledgedFutureReferral)
            yield return new ValidationResult(
                "You must acknowledge that your application will only be processed if your Local Authority takes part in the Warm Homes: Local Grant scheme",
                new[] { nameof(UserAcknowledgedFutureReferral) });
    }
}