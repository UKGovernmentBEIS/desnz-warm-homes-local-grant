using HerPublicWebsite.BusinessLogic.Models.Enums;
using Newtonsoft.Json;

namespace HerPublicWebsite.BusinessLogic.ExternalServices.EpbEpc;
    
public record class EpcDataDto
{
    [JsonProperty("assessment")]
    public EpcAssessmentDto Assessment { get; set; }
}

public record class EpbEpcDto
{
    [JsonProperty("data", Required = Required.Always)]
    public EpcDataDto Data { get; set; }
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
    public DateTime LodgementDate { get; set; }

    [JsonProperty("currentBand")]
    public EpcRating CurrentBand { get; set; }
}
