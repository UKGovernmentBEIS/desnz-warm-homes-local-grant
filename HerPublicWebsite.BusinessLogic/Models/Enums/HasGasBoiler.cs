using GovUkDesignSystem.Attributes;

namespace HerPublicWebsite.BusinessLogic.Models.Enums;

public enum HasGasBoiler
{
    [GovUkRadioCheckboxLabelText(Text = "No, I do not have a gas boiler")]
    No = 0,
    [GovUkRadioCheckboxLabelText(Text = "Yes, I have a gas boiler")]
    Yes = 1,
}
