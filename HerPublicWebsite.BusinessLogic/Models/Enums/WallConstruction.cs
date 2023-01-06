using GovUkDesignSystem.Attributes;

namespace HerPublicWebsite.BusinessLogic.Models.Enums
{
    public enum WallConstruction
    {
        [GovUkRadioCheckboxLabelText(Text = "Solid walls")]
        Solid,
        [GovUkRadioCheckboxLabelText(Text = "Cavity walls")]
        Cavity,
        [GovUkRadioCheckboxLabelText(Text = "Mix of solid and cavity walls")]
        Mixed,
        [GovUkRadioCheckboxLabelText(Text = "I don’t see my option listed")]
        Other,
        [GovUkRadioCheckboxLabelText(Text = "I don't know")]
        DoNotKnow,
    }
}