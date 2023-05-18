using HerPublicWebsite.BusinessLogic.Models.Enums;

namespace HerPublicWebsite.Extensions;

public static class EnumExtensions
{
    public static string ForDisplay(this PropertyType propertyType)
    {
        return propertyType switch
        {
            PropertyType.Bungalow => "Bungalow",
            PropertyType.House => "House",
            PropertyType.ApartmentFlatOrMaisonette => "Apartment, flat, or maisonette",
            _ => propertyType.ToString()
        };
    }
    
    public static string ForDisplay(this HouseType houseType)
    {
        return houseType switch
        {
            HouseType.EndTerrace => "End of terrace",
            HouseType.SemiDetached => "Semi detached",
            HouseType.Detached => "Detached",
            HouseType.Terraced => "Terraced",
            _ => houseType.ToString()
        };
    }
    
    public static string ForDisplay(this BungalowType bungalowType)
    {
        return bungalowType switch
        {
            BungalowType.EndTerrace => "End of terrace",
            BungalowType.SemiDetached => "Semi detached",
            BungalowType.Detached => "Detached",
            BungalowType.Terraced => "Terraced",
            _ => bungalowType.ToString()
        };
    }
    
    public static string ForDisplay(this FlatType flatType)
    {
        return flatType switch
        {
            FlatType.GroundFloor => "Ground floor or basement",
            FlatType.MiddleFloor => "Middle floor",
            FlatType.TopFloor => "Top floor",
            _ => flatType.ToString()
        };
    }
}
