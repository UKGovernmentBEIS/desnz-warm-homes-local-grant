using GovUkDesignSystem.Attributes.ValidationAttributes;
using HerPublicWebsite.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace HerPublicWebsite.Models.Questionnaire;

public class ConfirmationViewModel : QuestionFlowViewModel
{
    public string ReferenceCode { get; set; }
    public string LocalAuthorityName { get; set; }
    public string LocalAuthorityWebsite { get; set; }
    public bool LocalAuthorityIsLiveWithHug2 { get; set; }
    public bool RequestEmailAddress { get; set; }
    public string ConfirmationSentToEmailAddress { get; set; }

    [GovUkValidateRequiredIf(ErrorMessageIfMissing = "Select whether we can send you your confirmation details", IsRequiredPropertyName = nameof(RequestEmailAddress))]
    public YesOrNo? SendConfirmationDetails { get; set; }
    [GovUkValidateRequired(ErrorMessageIfMissing = "Select whether we can notify you about future energy grants")]
    public YesOrNo? CanNotifyAboutFutureSchemes { get; set; }

    [EmailAddress(ErrorMessage = "Enter an email address in the correct format, like name@example.com")]
    [GovUkValidateRequiredIf(ErrorMessageIfMissing = "Enter your email address for confirmation", IsRequiredPropertyName = nameof(ConfirmationEmailAddressRequired))]
    public string ConfirmationEmailAddress { get; set; }
    [EmailAddress(ErrorMessage = "Enter an email address in the correct format, like name@example.com")]
    [GovUkValidateRequiredIf(ErrorMessageIfMissing = "Enter your email address for future notifications", IsRequiredPropertyName = nameof(NotificationEmailAddressRequired))]
    public string NotificationEmailAddress { get; set; }

    public bool ConfirmationEmailAddressRequired => RequestEmailAddress && SendConfirmationDetails is YesOrNo.Yes;
    public bool NotificationEmailAddressRequired => RequestEmailAddress && CanNotifyAboutFutureSchemes is YesOrNo.Yes;

    public bool EmailPreferenceSubmitted { get; set; }
}
