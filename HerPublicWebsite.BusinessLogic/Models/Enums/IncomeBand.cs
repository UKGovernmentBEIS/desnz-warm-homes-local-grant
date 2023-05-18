using GovUkDesignSystem.Attributes;

namespace HerPublicWebsite.BusinessLogic.Models.Enums;

public enum IncomeBand
{
    [GovUkRadioCheckboxLabelText(Text = "£31,000 or less")]
    UnderOrEqualTo31000,
    [GovUkRadioCheckboxLabelText(Text = "More than £31,000")]
    GreaterThan31000
}
