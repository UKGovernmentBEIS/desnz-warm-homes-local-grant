using Newtonsoft.Json;

namespace HerPublicWebsite.BusinessLogic.ExternalServices.OsPlaces;

public class OsPlacesPostcodeResponseDto
{
    [JsonProperty("results")]
    public List<OsPlacesPostcodeResultDto> Results { get; set; }
    
    [JsonProperty("header")]
    public OsPlacesHeaderDto Header { get; set; }
}

public class OsPlacesHeaderDto
{
    [JsonProperty("totalresults")]
    public int TotalResults { get; set; }
}

public class OsPlacesPostcodeResultDto
{
    [JsonProperty("DPA")]
    public OsPlacesDpaDto Dpa { get; set; }
}
