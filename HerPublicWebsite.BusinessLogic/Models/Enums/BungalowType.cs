using GovUkDesignSystem.Attributes;

namespace HerPublicWebsite.BusinessLogic.Models.Enums
{
    public enum BungalowType
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