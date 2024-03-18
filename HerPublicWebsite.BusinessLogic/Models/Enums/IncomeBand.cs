using GovUkDesignSystem.Attributes;

namespace HerPublicWebsite.BusinessLogic.Models.Enums;

// WARNING: Do not re-order this enum without considering the impact on questionnaires stored
// in session data, and data that has been recorded in the database, as it may make that data
// invalid
public enum IncomeBand
{
    [Obsolete("This band is no longer used by any Local Authorities, but is kept to ensure backwards-compatibility")]
    [GovUkRadioCheckboxLabelText(Text = "£31,000 or less")]
    UnderOrEqualTo31000,
    [Obsolete("This band is no longer used by any Local Authorities, but is kept to ensure backwards-compatibility")]
    [GovUkRadioCheckboxLabelText(Text = "More than £31,000")]
    GreaterThan31000,
    [Obsolete("This band is no longer used by any Local Authorities, but is kept to ensure backwards-compatibility")]
    [GovUkRadioCheckboxLabelText(Text = "£34,500 or less")]
    UnderOrEqualTo34500,
    [Obsolete("This band is no longer used by any Local Authorities, but is kept to ensure backwards-compatibility")]
    [GovUkRadioCheckboxLabelText(Text = "More than £34,500")]
    GreaterThan34500,
    [GovUkRadioCheckboxLabelText(Text = "£36,000 or less")]
    UnderOrEqualTo36000,
    [GovUkRadioCheckboxLabelText(Text = "More than £36,000")]
    GreaterThan36000
}
