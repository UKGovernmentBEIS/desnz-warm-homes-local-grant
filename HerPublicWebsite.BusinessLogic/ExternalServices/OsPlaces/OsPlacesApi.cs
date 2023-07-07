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
            
            // Filter and parse DPA and LPI addresses
            var dpaAddresses = results
                .Where(r => r.Dpa is not null)
                .Select(r => r.Dpa.Parse());
            
            // The LPI dataset contains addresses that aren't residential properties, we want to filter those out
            var lpiAddresses = results
                .Where(r => r.Lpi is not null && r.Lpi.IsCurrentResidential())
                .Select(r => r.Lpi.Parse());

            // Join results based on UPRN
            var joinedAddresses = dpaAddresses.UnionBy(lpiAddresses, a => a.Uprn).ToList();

            var filteredResults = buildingNameOrNumber is null
                ? joinedAddresses
                : joinedAddresses.Where(a =>
                    a.AddressLine1.ToLower().Contains(buildingNameOrNumber.ToLower())
                    || a.AddressLine2.ToLower().Contains(buildingNameOrNumber.ToLower()))
                .ToList();

            // If the filter doesn't match then show all the results we found.
            if (!filteredResults.Any())
            {
                filteredResults = joinedAddresses;
            }
            
            return filteredResults.OrderBy(a => a.AddressLine1).ToList();
        }
        catch (Exception e) {
            logger.LogError("OS Places postcode request failed: {}", e.Message);
            return new List<Address>();
        }
    }

    private RequestParameters GetRequestParameters(string postcode, int? offset = null)
    {
        var path = $"/search/places/v1/postcode?maxresults={MaxResults}&postcode={postcode}&lr=EN&dataset=DPA,LPI";
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
