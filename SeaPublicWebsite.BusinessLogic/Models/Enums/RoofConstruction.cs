using GovUkDesignSystem.Attributes;

namespace SeaPublicWebsite.BusinessLogic.Models.Enums
{
    public enum RoofConstruction
    { 
        [GovUkRadioCheckboxLabelText(Text = "Yes, I only have a flat roof or roofs")]
        Flat,
        [GovUkRadioCheckboxLabelText(Text = "Yes, some of my roof is flat and some of my roof is pitched")]
        Mixed,
        [GovUkRadioCheckboxLabelText(Text = "No, I only have a pitched roof or roofs")]
        Pitched,
    }
}
