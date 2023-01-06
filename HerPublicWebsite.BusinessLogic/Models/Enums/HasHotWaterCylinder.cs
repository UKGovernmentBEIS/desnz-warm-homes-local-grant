using GovUkDesignSystem.Attributes;

namespace HerPublicWebsite.BusinessLogic.Models.Enums
{
    public enum HasHotWaterCylinder
    {
        [GovUkRadioCheckboxLabelText(Text = "Yes, I have a hot water cylinder")]
        Yes,
        [GovUkRadioCheckboxLabelText(Text = "No, I do not have a hot water cylinder")]
        No,
        [GovUkRadioCheckboxLabelText(Text = "I don't know")]
        DoNotKnow,
    }
}