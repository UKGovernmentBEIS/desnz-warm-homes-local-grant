using GovUkDesignSystem.Attributes;

namespace HerPublicWebsite.BusinessLogic.Models.Enums
{
    public enum HeatingType
    {
        [GovUkRadioCheckboxLabelText(Text = "Gas boiler")]
        GasBoiler,
        [GovUkRadioCheckboxLabelText(Text = "Oil boiler")]
        OilBoiler,
        [GovUkRadioCheckboxLabelText(Text = "LPG boiler")]
        LpgBoiler,
        [GovUkRadioCheckboxLabelText(Text = "Storage heaters")]
        Storage,
        [GovUkRadioCheckboxLabelText(Text = "Direct acting electric heating")]
        DirectActionElectric,
        [GovUkRadioCheckboxLabelText(Text = "Heat pump")]
        HeatPump,
        [GovUkRadioCheckboxLabelText(Text = "Other")]
        Other,
        [GovUkRadioCheckboxLabelText(Text = "I don't know")]
        DoNotKnow,
    }
}
