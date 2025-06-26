using GovUkDesignSystem.Attributes.ValidationAttributes;
using System.ComponentModel.DataAnnotations;
using WhlgPublicWebsite.Models.Enums;

namespace WhlgPublicWebsite.Models.Questionnaire;

public class NoFundingViewModel : QuestionFlowViewModel
{
    public string LocalAuthorityName { get; set; }

    [EmailAddress(ErrorMessage = "Enter an email address in the correct format, like name@example.com")]
    [GovUkValidateRequiredIf(ErrorMessageIfMissing = "Enter your email address",
        IsRequiredPropertyName = nameof(IsEmailAddressRequired))]
    public string EmailAddress { get; set; }

    [GovUkValidateRequired(ErrorMessageIfMissing = "Select whether you would like to be contacted about future grants")]
    public YesOrNo? CanContactByEmailAboutFutureSchemes { get; set; }

    public bool IsEmailAddressRequired => CanContactByEmailAboutFutureSchemes is YesOrNo.Yes;

    public bool Submitted { get; set; }
}