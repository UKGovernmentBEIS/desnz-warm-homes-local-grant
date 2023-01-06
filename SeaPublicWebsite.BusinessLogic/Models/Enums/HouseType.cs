using GovUkDesignSystem;
using GovUkDesignSystem.Attributes;
using GovUkDesignSystem.Attributes.ValidationAttributes;

namespace SeaPublicWebsite.BusinessLogic.Models.Enums
{
    public enum HouseType
    {
        [GovUkRadioCheckboxLabelText(Text = "Detached")]
        Detached,
        [GovUkRadioCheckboxLabelText(Text = "Semi-detached")]
        SemiDetached,
        [GovUkRadioCheckboxLabelText(Text = "Terraced")]
        Terraced,
        [GovUkRadioCheckboxLabelText(Text = "End terrace")]
        EndTerrace
    }
}