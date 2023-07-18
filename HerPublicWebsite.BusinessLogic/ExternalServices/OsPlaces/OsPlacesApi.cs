using System.Text.RegularExpressions;
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
    private readonly Regex NonAlphanumericCharacters = new Regex(@"[^a-zA-Z0-9]", RegexOptions.Compiled);

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
            
            // Filter out addresses that aren't residential properties from the LPI dataset
            var lpiAddresses = results
                .Where(r => r.Lpi is not null && r.Lpi.IsCurrentResidential())
                .Select(r => r.Lpi.Parse())
                .ToList();

            // We use LPI as the best source for which addresses exist, but use the DPA data for a property if it has
            // a DPA entry.
            var uprnsToUse = lpiAddresses.Select(la => la.Uprn).ToList();
            
            // Parse and filter DPA addresses
            var dpaAddresses = results
                .Where(r => r.Dpa is not null && uprnsToUse.Contains(r.Dpa.Uprn))
                .Select(r => r.Dpa.Parse())
                .ToList();

            var dpaUprns = dpaAddresses.Select(da => da.Uprn).ToList();

            var joinedAddresses = dpaAddresses.Concat(lpiAddresses.Where(la => !dpaUprns.Contains(la.Uprn))).ToList();

            // Filter by the building name or number the user provided.
            // Some users may fill in the full street address (e.g. 45 High Street), we'd like to match that if possible
            var userFilterParts = NonAlphanumericCharacters
                .Replace((buildingNameOrNumber ?? "").ToLower(), " ")
                .Split(' ');

            var filteredResults = userFilterParts.Length == 0
                ? joinedAddresses
                : joinedAddresses.Where(a => 
                        userFilterParts.All(userFilterPart =>
                            a.AddressLine1.ToLower().Contains(userFilterPart) ||
                            a.AddressLine2.ToLower().Contains(userFilterPart)))
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
