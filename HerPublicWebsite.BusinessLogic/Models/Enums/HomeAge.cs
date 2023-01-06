using GovUkDesignSystem.Attributes;

namespace HerPublicWebsite.BusinessLogic.Models.Enums
{
    public enum HomeAge
    {
        [GovUkRadioCheckboxLabelText(Text = "Before 1900")]
        Pre1900,
        [GovUkRadioCheckboxLabelText(Text = "1900 to 1929")]
        From1900To1929,
        [GovUkRadioCheckboxLabelText(Text = "1930 to 1949")]
        From1930To1949,
        [GovUkRadioCheckboxLabelText(Text = "1950 to 1966")]
        From1950To1966,
        [GovUkRadioCheckboxLabelText(Text = "1967 to 1975")]
        From1967To1975,
        [GovUkRadioCheckboxLabelText(Text = "1976 to 1982")]
        From1976To1982,
        [GovUkRadioCheckboxLabelText(Text = "1983 to 1990")]
        From1983To1990,
        [GovUkRadioCheckboxLabelText(Text = "1991 to 1995")]
        From1991To1995,
        [GovUkRadioCheckboxLabelText(Text = "1996 to 2002")]
        From1996To2002,
        [GovUkRadioCheckboxLabelText(Text = "2003 to 2006")]
        From2003To2006,
        [GovUkRadioCheckboxLabelText(Text = "2007 to 2011")]
        From2007To2011,
        [GovUkRadioCheckboxLabelText(Text = "2012 or newer")]
        From2012ToPresent
    }
}