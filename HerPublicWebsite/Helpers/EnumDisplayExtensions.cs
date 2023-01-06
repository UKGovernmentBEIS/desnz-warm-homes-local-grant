using GovUkDesignSystem.Attributes;
using HerPublicWebsite.BusinessLogic.Models.Enums;

namespace HerPublicWebsite.Helpers;

public static class EnumDisplayExtensions
{
    public static string DescriptionForAnswerSummary(this FloorConstruction? floorConstruction)
    {
        return floorConstruction is FloorConstruction.Mix
            ? "A mix of both suspended timber and solid concrete"
            : GovUkRadioCheckboxLabelTextAttribute.GetLabelText(floorConstruction);
    }
}