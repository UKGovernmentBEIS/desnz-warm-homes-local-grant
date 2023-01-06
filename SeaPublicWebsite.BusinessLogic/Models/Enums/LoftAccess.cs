using GovUkDesignSystem.Attributes;

namespace SeaPublicWebsite.BusinessLogic.Models.Enums
{
    public enum LoftAccess
    {
        [GovUkRadioCheckboxLabelText(Text = "Yes, there is access to my loft")]
        Yes,
        [GovUkRadioCheckboxLabelText(Text= "No, there is no access to my loft")]
        No,
    }
}