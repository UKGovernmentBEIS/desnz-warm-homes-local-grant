using GovUkDesignSystem.Attributes;

namespace HerPublicWebsite.BusinessLogic.Models.Enums
{
    // WARNING: Do not re-order this enum without considering the impact on questionnaires stored
    // in session data, and data that has been recorded in the database, as it may make that data
    // invalid
    public enum OwnershipStatus
    {
        [GovUkRadioCheckboxLabelText(Text = "Yes, I own my property and live in it")]
        OwnerOccupancy,
        [GovUkRadioCheckboxLabelText(Text = "No, I am a tenant or social housing tenant")]
        PrivateTenancy,
        [GovUkRadioCheckboxLabelText(Text = "I am the property owner but lease my property to one or more tenants")]
        Landlord,
    }
}
