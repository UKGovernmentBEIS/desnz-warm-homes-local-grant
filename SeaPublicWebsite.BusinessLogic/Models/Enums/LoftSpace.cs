using GovUkDesignSystem.Attributes;

namespace SeaPublicWebsite.BusinessLogic.Models.Enums
{
    public enum LoftSpace
    {
        [GovUkRadioCheckboxLabelText(Text = "Yes, I have a loft that hasn't been converted into a room")]
        Yes,
        [GovUkRadioCheckboxLabelText(Text= "No, I don’t have a loft or my loft has been converted into a room")]
        No,
    }
}