using System.Text.RegularExpressions;
using WhlgPublicWebsite.BusinessLogic.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WhlgPublicWebsite.BusinessLogic.ExternalServices.Common;
using WhlgPublicWebsite.BusinessLogic.Models;

namespace WhlgPublicWebsite.BusinessLogic.ExternalServices.OsPlaces;

public class OsPlacesApi(IOptions<OsPlacesConfiguration> options, ILogger<OsPlacesApi> logger)
    : IOsPlacesApi
{
    private readonly OsPlacesConfiguration config = options.Value;
    private const int MaxResults = 100;
    private readonly Regex nonAlphanumericCharacters = new(@"[^a-zA-Z0-9]", RegexOptions.Compiled);

    public async Task<List<Address>> GetAddressesAsync(string postcode, string buildingNameOrNumber)
    {
        if (!postcode.IsValidUkPostcodeFormat())
        {
            return new List<Address>();
        }

        var parameters = GetRequestParameters(postcode);

        try
        {
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
                .Select(r => r.Lpi)
                .ToList();

            // Sort LPI addresses
            lpiAddresses.Sort();

            var joinedAddresses = lpiAddresses
                .Select(lpiAddress =>
                {
                    var matchingResult = results.FirstOrDefault(r => r.Dpa != null && r.Dpa.Uprn == lpiAddress.Uprn);
                    return matchingResult != null ? matchingResult.Dpa.Parse() : lpiAddress.Parse();
                })
                .ToList();


            // Filter by the building name or number the user provided.
            // Some users may fill in the full street address (e.g. 45 High Street), we'd like to match that if possible
            var userFilterParts = nonAlphanumericCharacters
                .Replace((buildingNameOrNumber ?? "").ToLower(), " ")
                .Split(' ');

            var filteredResults = userFilterParts.Length == 0
                ? joinedAddresses
                : joinedAddresses.Where(a =>
                        userFilterParts.All(userFilterPart =>
                            a.AddressLine1.Contains(userFilterPart, StringComparison.CurrentCultureIgnoreCase) ||
                            a.AddressLine2.Contains(userFilterPart, StringComparison.CurrentCultureIgnoreCase)))
                    .ToList();

            // If the filter doesn't match then show all the results we found.
            if (!filteredResults.Any())
            {
                filteredResults = joinedAddresses;
            }

            return filteredResults;
        }
        catch (Exception e)
        {
            logger.LogError("OS Places postcode request failed: {}", e.Message);
            return [];
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