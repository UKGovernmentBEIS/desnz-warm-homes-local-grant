using HerPublicWebsite.BusinessLogic.Models;
using HerPublicWebsite.BusinessLogic.Models.Enums;
using Newtonsoft.Json;

namespace HerPublicWebsite.BusinessLogic.ExternalServices.EpbEpc;

public record class EpbEpcDto
{
    [JsonProperty("data", Required = Required.Always)]
    public EpcDataDto Data { get; set; }
}

public record class EpcDataDto
{
    [JsonProperty("assessment")]
    public EpcAssessmentDto Assessment { get; set; }
}

public record class EpcAddressDto
{
    [JsonProperty("addressLine1")]
    public string AddressLine1 { get; set; }

    [JsonProperty("addressLine2")]
    public string AddressLine2 { get; set; }

    [JsonProperty("addressLine3")]
    public string AddressLine3 { get; set; }

    [JsonProperty("addressLine4")]
    public string AddressLine4 { get; set; }

    [JsonProperty("town")]
    public string Town { get; set; }

    [JsonProperty("postcode")]
    public string Postcode { get; set; }
}

public record class EpcAssessmentDto
{
    [JsonProperty("address")]
    public EpcAddressDto Address { get; set; }

    [JsonProperty("uprn")]
    public string Uprn { get; set; }

    [JsonProperty("lodgementDate")]
    public string LodgementDate { get; set; }
    
    [JsonProperty("expiryDate")]
    public string ExpiryDate { get; set; }

    [JsonProperty("currentBand")]
    public string CurrentBand { get; set; }
    
    [JsonProperty("propertyType")]
    public string PropertyType { get; set; }
    
    [JsonProperty("builtForm")]
    public string BuiltForm { get; set; }

    public EpcDetails Parse()
    {
        return new EpcDetails
        {
            AddressLine1 = Address?.AddressLine1,
            AddressLine2 = Address?.AddressLine2,
            AddressLine3 = Address?.AddressLine3,
            AddressLine4 = Address?.AddressLine4,
            AddressTown = Address?.Town,
            AddressPostcode = Address?.Postcode,
            LodgementDate = ParseDate(LodgementDate),
            ExpiryDate = ParseDate(ExpiryDate),
            EpcRating = ParseBand(),
            PropertyType = ParsePropertyType(),
            HouseType = ParseHouseType(),
            BungalowType = ParseBungalowType(),
            FlatType = ParseFlatType(),
        };
    }
    
    private DateTime? ParseDate(string dateString)
    {
        if (dateString is null)
        {
            return null;
        }
        
        var date = DateTime.Parse(dateString);
        return DateTime.SpecifyKind(date, DateTimeKind.Utc);
    }
    
    private EpcRating ParseBand()
    {
        return CurrentBand switch
        {
            "A" => EpcRating.A,
            "B" => EpcRating.B,
            "C" => EpcRating.C,
            "D" => EpcRating.D,
            "E" => EpcRating.E,
            "F" => EpcRating.F,
            "G" => EpcRating.G,
            _ => EpcRating.Unknown
        };
    }
    
    private PropertyType? ParsePropertyType()
    {
        if (PropertyType is null)
        {
            return null;
        }
        
        if (PropertyType.Contains("House", StringComparison.OrdinalIgnoreCase))
        {
            return HerPublicWebsite.BusinessLogic.Models.Enums.PropertyType.House;
        }

        if (PropertyType.Contains("Bungalow", StringComparison.OrdinalIgnoreCase))
        {
            return HerPublicWebsite.BusinessLogic.Models.Enums.PropertyType.Bungalow;
        }

        if (PropertyType.Contains("Flat", StringComparison.OrdinalIgnoreCase) ||
            PropertyType.Contains("Maisonette", StringComparison.OrdinalIgnoreCase))
        {
            return HerPublicWebsite.BusinessLogic.Models.Enums.PropertyType.ApartmentFlatOrMaisonette;
        }

        return null;
    }
    
    private HouseType? ParseHouseType()
    {
        if (ParsePropertyType() is not HerPublicWebsite.BusinessLogic.Models.Enums.PropertyType.House)
        {
            return null;
        }

        if (PropertyType.Contains("semi-detached", StringComparison.OrdinalIgnoreCase))
        {
            return HouseType.SemiDetached;
        }
        
        if (PropertyType.Contains("detached", StringComparison.OrdinalIgnoreCase))
        {
            return HouseType.Detached;
        }

        if (PropertyType.Contains("mid-terrace", StringComparison.OrdinalIgnoreCase))
        {
            return HouseType.Terraced;
        }
        
        if (PropertyType.Contains("end-terrace", StringComparison.OrdinalIgnoreCase))
        {
            return HouseType.EndTerrace;
        }

        return null;
    }
    
    private BungalowType? ParseBungalowType()
    {
        if (ParsePropertyType() is not HerPublicWebsite.BusinessLogic.Models.Enums.PropertyType.Bungalow)
        {
            return null;
        }

        if (PropertyType.Contains("semi-detached", StringComparison.OrdinalIgnoreCase))
        {
            return BungalowType.SemiDetached;
        }
        
        if (PropertyType.Contains("detached", StringComparison.OrdinalIgnoreCase))
        {
            return BungalowType.Detached;
        }

        if (PropertyType.Contains("mid-terrace", StringComparison.OrdinalIgnoreCase))
        {
            return BungalowType.Terraced;
        }
        
        if (PropertyType.Contains("end-terrace", StringComparison.OrdinalIgnoreCase))
        {
            return BungalowType.EndTerrace;
        }

        return null;
    }
    
    private FlatType? ParseFlatType()
    {
        if (ParsePropertyType() is not HerPublicWebsite.BusinessLogic.Models.Enums.PropertyType.ApartmentFlatOrMaisonette)
        {
            return null;
        }
        
        if (PropertyType.Contains("basement", StringComparison.OrdinalIgnoreCase) ||
            PropertyType.Contains("ground", StringComparison.OrdinalIgnoreCase))
        {
            return FlatType.GroundFloor;
        }

        if (PropertyType.Contains("mid", StringComparison.OrdinalIgnoreCase))
        {
            return FlatType.MiddleFloor;
        }
        
        if (PropertyType.Contains("top", StringComparison.OrdinalIgnoreCase))
        {
            return FlatType.TopFloor;
        }

        return null;
    }
}
