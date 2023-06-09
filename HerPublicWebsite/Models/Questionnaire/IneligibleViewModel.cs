using GovUkDesignSystem.Attributes.ValidationAttributes;
using HerPublicWebsite.Models.Enums;
using System.ComponentModel.DataAnnotations;

using IncomeBandEnum = HerPublicWebsite.BusinessLogic.Models.Enums.IncomeBand;

namespace HerPublicWebsite.Models.Questionnaire;

public class IneligibleViewModel : QuestionFlowViewModel
{
    [GovUkValidateRequired(ErrorMessageIfMissing = "Select whether you would like to be contacted about future grants")]
    public YesOrNo? CanContactByEmailAboutFutureSchemes { get; set; }

    [EmailAddress(ErrorMessage = "Enter an email address in the correct format, like name@example.com")]
    [GovUkValidateRequiredIf(ErrorMessageIfMissing = "Enter your email address", IsRequiredPropertyName = nameof(IsEmailAddressRequired))]
    public string EmailAddress { get; set; }

    public bool IsEmailAddressRequired => CanContactByEmailAboutFutureSchemes is YesOrNo.Yes;

    public bool EpcIsTooHigh { get; set; }

    public bool IncomeIsTooHigh { get; set; }

    public string LocalAuthorityName { get; set; }

    public string LocalAuthorityWebsite { get; set; }

    public bool Submitted { get; set; }
}
