using GovUkDesignSystem.Attributes.ValidationAttributes;
using HerPublicWebsite.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace HerPublicWebsite.Models.Questionnaire;

public class EligibleViewModel : QuestionFlowViewModel
{
    [GovUkValidateRequired(ErrorMessageIfMissing = "Enter your full name")]
    public string Name { get; set; }
    [GovUkValidateRequired(ErrorMessageIfMissing = "Select whether they can contact you by email")]
    public YesOrNo? CanContactByEmail { get; set; }
    [EmailAddress(ErrorMessage = "Enter an email address in the correct format, like name@example.com")]
    [GovUkValidateRequiredIf(ErrorMessageIfMissing = "Enter your email address", IsRequiredPropertyName = nameof(IsEmailAddressRequired))]
    public string EmailAddress { get; set; }
    [GovUkValidateRequired(ErrorMessageIfMissing = "Select whether they can contact you by phone")]
    public YesOrNo? CanContactByPhone { get; set; }
    [GovUkValidateRequiredIf(ErrorMessageIfMissing = "Enter your phone number", IsRequiredPropertyName = nameof(IsPhoneRequired))]
    public string Telephone { get; set; }

    public string LocalAuthorityName { get; set; }
    public bool LocalAuthorityIsLiveWithHug2 { get; set; }

    public bool IsEmailAddressRequired => CanContactByEmail is YesOrNo.Yes;
    public bool IsPhoneRequired => CanContactByPhone is YesOrNo.Yes;
}
