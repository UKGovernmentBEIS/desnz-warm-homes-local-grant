using System.ComponentModel.DataAnnotations;
using GovUkDesignSystem.Attributes.ValidationAttributes;

namespace WhlgPublicWebsite.Models.Questionnaire;

public class FutureContactChannelsViewModel : QuestionFlowViewModel
{
    public bool ContactByEmail { get; set; }
    public bool ContactByPhone { get; set; }
    public bool ContactBySms { get; set; }

    [GovUkValidateRequired(ErrorMessageIfMissing = "Enter your name")]
    public string Name { get; set; }

    [EmailAddress(ErrorMessage = "Enter an email address in the correct format, like name@example.com")]
    [GovUkValidateRequiredIf(ErrorMessageIfMissing = "Enter your email address", IsRequiredPropertyName = nameof(EmailRequired))]
    public string Email { get; set; }

    [Phone(ErrorMessage = "Enter a phone number in the correct format")]
    [GovUkValidateRequiredIf(ErrorMessageIfMissing = "Enter your phone number", IsRequiredPropertyName = nameof(PhoneRequired))]
    public string PhoneNumber { get; set; }

    public bool AtLeastOneChannelRequired { get; set; }

    public bool EmailRequired => ContactByEmail;
    public bool PhoneRequired => ContactByPhone || ContactBySms;
}
