using GovUkDesignSystem.Attributes;

namespace HerPublicWebsite.BusinessLogic.Models.Enums;

// WARNING: Do not re-order this enum without considering the impact on questionnaires stored
// in session data, and data that has been recorded in the database, as it may make that data
// invalid
public enum EpcConfirmation
{
    Yes = 0,
    No = 1,
    [GovUkRadioCheckboxLabelText(Text = "I don't know")]
    Unknown
}
