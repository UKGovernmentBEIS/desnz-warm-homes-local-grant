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
    private const int MaxResults = 100;

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
        
        var parameters = GetRequestParameters(postcode);

        try {
            var response = await HttpRequestHelper.SendGetRequestAsync<OsPlacesPostcodeResponseDto>(parameters);

            var results = response.Results ?? new List<OsPlacesPostcodeResultDto>();

            var totalNumberOfResultsFound = response.Header.TotalResults;

            if (totalNumberOfResultsFound > MaxResults)
            {
                var resultsRequested = MaxResults;
                while (resultsRequested < totalNumberOfResultsFound)
                {
                    parameters = GetRequestParameters(postcode, resultsRequested);
                    response = await HttpRequestHelper.SendGetRequestAsync<OsPlacesPostcodeResponseDto>(parameters);
                    results = results.Concat(response.Results).ToList();
                    resultsRequested += MaxResults;
                }
            }

            var filteredResults = buildingNameOrNumber is null
                ? results
                : results.Where(r =>
                    r.Dpa.BuildingNumber == buildingNameOrNumber
                    || r.Dpa.BuildingName?.ToLower() == buildingNameOrNumber.ToLower()
                    || r.Dpa.SubBuildingName?.ToLower() == buildingNameOrNumber.ToLower())
                .ToList();

            // If the filter doesn't match then show all the results we found.
            if (!filteredResults.Any())
            {
                filteredResults = results;
            }
            
            return filteredResults.Select(r => r.Dpa.Parse()).ToList();
        }
        catch (Exception e) {
            logger.LogError("OS Places postcode request failed: {}", e.Message);
            return new List<Address>();
        }
    }

    private RequestParameters GetRequestParameters(string postcode, int? offset = null)
    {
        var path = $"/search/places/v1/postcode?maxresults={MaxResults}&postcode={postcode}&lr=EN";
        if (offset is not null)
        {
            path += $"&offset={offset}";
        }
        
        return new RequestParameters
        {
            BaseAddress = config.BaseUrl,
            Path = path,
            Headers = new Dictionary<string, string> { { "Key", config.Key } }
        };
    }
}
