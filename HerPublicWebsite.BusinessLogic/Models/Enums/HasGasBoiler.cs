using GovUkDesignSystem.Attributes;

namespace HerPublicWebsite.BusinessLogic.Models.Enums;

public enum HasGasBoiler
{
    No = 0,
    Yes = 1,
    [GovUkRadioCheckboxLabelText(Text = "I do not know")]
    Unknown = 2
}
