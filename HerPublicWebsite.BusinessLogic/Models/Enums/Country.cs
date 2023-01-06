using GovUkDesignSystem.Attributes;

namespace HerPublicWebsite.BusinessLogic.Models.Enums
{
    public enum Country
    {
        England,
        Wales,
        Scotland,
        [GovUkRadioCheckboxLabelText(Text = "Northern Ireland")]
        NorthernIreland,
        [GovUkRadioCheckboxLabelText(Text = "A country not listed here")]
        Other
    }
}
