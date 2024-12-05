using System;
using GovUkDesignSystem.Attributes.ValidationAttributes;
using HerPublicWebsite.Models.Enums;

namespace HerPublicWebsite.Models.ReferralRequestFollowUp;

public class ReferralRequestFollowUpResponsePageViewModel
{
    public string Token { get; set; }
    public string ReferralCode {get; set;}
    public DateTime RequestDate {get; set;}
    [GovUkValidateRequired(ErrorMessageIfMissing = "Select a response")]
    public YesOrNo? HasFollowedUp { get; set; }
}
