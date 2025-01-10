using System.ComponentModel.DataAnnotations;
using GovUkDesignSystem.Attributes.ValidationAttributes;
using WhlgPublicWebsite.Helpers;
using WhlgPublicWebsite.Models.Enums;

namespace WhlgPublicWebsite.Models.Questionnaire;

public class EligibleViewModel : QuestionFlowViewModel
{
    [GovUkValidateRequired(ErrorMessageIfMissing = "Enter your full name")]
    public string Name { get; set; }

    [GovUkValidateRequired(ErrorMessageIfMissing = "Select whether they can contact you by email")]
    public YesOrNo? CanContactByEmail { get; set; }

    [EmailAddress(ErrorMessage = "Enter an email address in the correct format, like name@example.com")]
    [GovUkValidateRequiredIf(ErrorMessageIfMissing = "Enter your email address",
        IsRequiredPropertyName = nameof(IsEmailAddressRequired))]
    public string EmailAddress { get; set; }

    [GovUkValidateRequired(ErrorMessageIfMissing = "Select whether they can contact you by phone")]
    public YesOrNo? CanContactByPhone { get; set; }

    [ValidUkPhoneNumber(ValidateIf = nameof(IsPhoneRequired),
        ErrorMessage =
            "Enter a telephone number, like 01632 960 001, 07700 900 982 or +44 808 157 0192")] // examples from https://design-system.service.gov.uk/patterns/telephone-numbers/
    [GovUkValidateRequiredIf(ErrorMessageIfMissing = "Enter your telephone number",
        IsRequiredPropertyName = nameof(IsPhoneRequired))]
    public string Telephone { get; set; }

    public string LocalAuthorityName { get; set; }

    public bool LocalAuthorityIsTakingFutureReferrals { get; set; }
    public bool LocalAuthorityIsLive { get; set; }
    public bool LocalAuthorityIsPending { get; set; }

    public bool IsEmailAddressRequired => CanContactByEmail is YesOrNo.Yes;
    public bool IsPhoneRequired => CanContactByPhone is YesOrNo.Yes;
}