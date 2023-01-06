using GovUkDesignSystem.Attributes;

namespace HerPublicWebsite.BusinessLogic.Models.Enums
{
    public enum CavityWallsInsulated
    {
        [GovUkRadioCheckboxLabelText(Text = "Yes, they are all insulated")]
        All,
        [GovUkRadioCheckboxLabelText(Text = "Some are insulated and some not")]
        Some,
        [GovUkRadioCheckboxLabelText(Text = "No, they are not insulated")]
        No,
        [GovUkRadioCheckboxLabelText(Text = "I don't know")]
        DoNotKnow, 
    }
}