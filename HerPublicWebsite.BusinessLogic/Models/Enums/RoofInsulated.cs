using GovUkDesignSystem.Attributes;

namespace HerPublicWebsite.BusinessLogic.Models.Enums
{
    public enum RoofInsulated
    {
        [GovUkRadioCheckboxLabelText(Text = "Yes, there is at least 200mm of insulation in my loft")]
        Yes,
        [GovUkRadioCheckboxLabelText(Text = "No, there is less than 200mm of insulation in my loft")]
        No,
        [GovUkRadioCheckboxLabelText(Text = "I don't know")]
        DoNotKnow,
    }
}