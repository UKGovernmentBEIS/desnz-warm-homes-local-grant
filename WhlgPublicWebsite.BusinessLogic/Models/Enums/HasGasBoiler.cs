using GovUkDesignSystem.Attributes;

namespace WhlgPublicWebsite.BusinessLogic.Models.Enums;

// WARNING: Do not re-order this enum without considering the impact on questionnaires stored
// in session data, and data that has been recorded in the database, as it may make that data
// invalid
public enum HasGasBoiler
{
    [GovUkRadioCheckboxLabelText(Text = "No, I do not have a mains gas boiler")]
    No = 0,
    [GovUkRadioCheckboxLabelText(Text = "Yes, I have a mains gas boiler")]
    Yes = 1,
}
