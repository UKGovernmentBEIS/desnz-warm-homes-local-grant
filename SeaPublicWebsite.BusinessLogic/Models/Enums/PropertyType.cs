using GovUkDesignSystem.Attributes;

namespace SeaPublicWebsite.BusinessLogic.Models.Enums
{
    public enum PropertyType
    {
        [GovUkRadioCheckboxLabelText(Text = "House")]
        House,
        [GovUkRadioCheckboxLabelText(Text = "Bungalow")]
        Bungalow,
        [GovUkRadioCheckboxLabelText(Text = "Apartment, flat or maisonette")]
        ApartmentFlatOrMaisonette
    }
}