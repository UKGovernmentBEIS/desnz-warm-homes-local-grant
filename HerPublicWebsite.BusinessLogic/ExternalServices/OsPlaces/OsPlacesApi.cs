using HerPublicWebsite.BusinessLogic.Extensions;
using HerPublicWebsite.BusinessLogic.ExternalServices.Common;
using HerPublicWebsite.BusinessLogic.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HerPublicWebsite.BusinessLogic.ExternalServices.OsPlaces;

public class OsPlacesApi : IOsPlacesApi
{
    private readonly OsPlacesConfiguration config;
    private readonly ILogger<OsPlacesApi> logger;

    public OsPlacesApi(IOptions<OsPlacesConfiguration> options, ILogger<OsPlacesApi> logger)
    {
        this.config = options.Value;
        this.logger = logger;
    }
    
    public async Task<List<Address>> GetAddressesAsync(string postcode, string buildingNameOrNumber)
    {
        if (!postcode.IsValidUkPostcodeFormat())
        {
            return new List<Address>();
        }
        
        var parameters = new RequestParameters
        {
            BaseAddress = config.BaseUrl,
            Path = $"/search/places/v1/postcode?postcode={postcode}",
            Headers = new Dictionary<string, string> { { "Key", config.Key } }
        };

        try {
            var response = await HttpRequestHelper.SendGetRequestAsync<OsPlacesPostcodeResponseDto>(parameters);

            var filteredResults = response.Results.Where(r => r.Dpa.BuildingNumber == buildingNameOrNumber || r.Dpa.BuildingName?.ToLower() == buildingNameOrNumber.ToLower()).ToList();

            // If the filter doesn't match then show all the results we found.
            if (!filteredResults.Any())
            {
                filteredResults = response.Results;
            }
            
            return filteredResults.Select(r => r.Dpa.Parse()).ToList();
        }
        catch (Exception e) {
            logger.LogError("OS Places postcode request failed: {}", e.Message);
            throw;
        }
    }
}
