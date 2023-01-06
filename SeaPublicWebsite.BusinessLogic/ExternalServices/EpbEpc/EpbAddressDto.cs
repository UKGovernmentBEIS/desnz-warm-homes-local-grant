using Newtonsoft.Json;

namespace SeaPublicWebsite.BusinessLogic.ExternalServices.EpbEpc;

public class EpbAddressDto
{
    [JsonProperty(PropertyName = "addressLine1")]
    public string Address1 { get; set; }
        
    [JsonProperty(PropertyName = "addressLine2")]
    public string Address2 { get; set; }
        
    [JsonProperty(PropertyName = "addressLine3")]
    public string Address3 { get; set; }
        
    [JsonProperty(PropertyName = "addressLine4")]
    public string Address4 { get; set; }
        
    [JsonProperty(PropertyName = "town")]
    public string Town { get; set; }
        
    [JsonProperty(PropertyName = "postcode")]
    public string Postcode { get; set; }
}