using GovUkDesignSystem.Attributes;

namespace SeaPublicWebsite.BusinessLogic.Models.Enums;

public enum YearBuilt
{
    [GovUkRadioCheckboxLabelText(Text = "Before 1930")]
    Pre1930,
    [GovUkRadioCheckboxLabelText(Text = "1930 to 1966")]
    From1930To1966,
    [GovUkRadioCheckboxLabelText(Text = "1967 to 1982")]
    From1967To1982,
    [GovUkRadioCheckboxLabelText(Text = "1983 to 1995")]
    From1983To1995,
    [GovUkRadioCheckboxLabelText(Text = "1996 to 2011")]
    From1996To2011,
    [GovUkRadioCheckboxLabelText(Text = "2012 or newer")]
    From2012ToPresent,
    [GovUkRadioCheckboxLabelText(Text = "I don't know")]
    DoNotKnow
}