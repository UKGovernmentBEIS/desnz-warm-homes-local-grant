using GovUkDesignSystem.Attributes;

namespace HerPublicWebsite.BusinessLogic.Models.Enums;

// WARNING: Do not re-order this enum without considering the impact on questionnaires stored
// in session data, and data that has been recorded in the database, as it may make that data
// invalid
public enum IncomeBand
{
    [GovUkRadioCheckboxLabelText(Text = "£31,000 or less")]
    UnderOrEqualTo31000,
    [GovUkRadioCheckboxLabelText(Text = "More than £31,000")]
    GreaterThan31000
}
