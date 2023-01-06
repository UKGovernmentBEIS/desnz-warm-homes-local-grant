using System.Net;
using System.Net.Http.Headers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using HerPublicWebsite.BusinessLogic.ExternalServices.Common;
using HerPublicWebsite.BusinessLogic.Models;

namespace HerPublicWebsite.BusinessLogic.ExternalServices.EpbEpc
{
    public class EpbEpcApi : IEpcApi
    {
        private readonly IMemoryCache memoryCache;
        private readonly EpbEpcConfiguration configuration;
        private readonly ILogger<EpbEpcApi> logger;
        private readonly string cacheTokenKey = "EpbEpcToken";

        public EpbEpcApi(IOptions<EpbEpcConfiguration> options, IMemoryCache memoryCache, ILogger<EpbEpcApi> logger)
        {
            this.memoryCache = memoryCache;
            this.configuration = options.Value;
            this.logger = logger;
        }

        public async Task<List<EpcSearchResult>> GetEpcsInformationForPostcodeAndBuildingNameOrNumber(string postcode,
            string buildingNameOrNumber = null)
        {
            var token = await RequestTokenIfNeeded();
            var query = $"postcode={postcode}";
            if (buildingNameOrNumber is not null)
            {
                query += $"&buildingNameOrNumber={buildingNameOrNumber}";
            }

            EpbAssessmentsDto response = null;
            try
            {
                response = await HttpRequestHelper.SendGetRequestAsync<EpbAssessmentsDto>(
                    new RequestParameters
                    {
                        BaseAddress = configuration.BaseUrl,
                        Path = $"/api/assessments/domestic-epcs/search?{query}",
                        Auth = new AuthenticationHeaderValue("Bearer", token)
                    });
            }
            catch (ApiException e)
            {
                if (e.StatusCode is not HttpStatusCode.NotFound)
                {
                    logger.LogError("There was an error sending a request to the epc api: {}", e.Message);
                }

                return new List<EpcSearchResult>();
            }

            var epcsInformation = response.Data.Assessments.Select(epcInfo => new EpcSearchResult(
                epcInfo.EpcId,
                epcInfo.Address.Address1,
                epcInfo.Address.Address2,
                epcInfo.Address.Postcode)).ToList();

            return EpcSearchResult.SortEpcsInformation(epcsInformation);
        }

        public async Task<Epc> GetEpcForId(string epcId)
        {
            var token = await RequestTokenIfNeeded();
            EpbEpcDto response = null;
            try
            {
                response = await HttpRequestHelper.SendGetRequestAsync<EpbEpcDto>(
                    new RequestParameters
                    {
                        BaseAddress = configuration.BaseUrl,
                        Path = $"/api/retrofit-advice/assessments/{epcId}",
                        Auth = new AuthenticationHeaderValue("Bearer", token)
                    });
            }
            catch (ApiException e)
            {
                logger.Log(LogLevel.Warning, "{Message}", e.Message);
                return null;
            }

            var epc = response.Data.Assessment;

            return epc.Parse();
        }

        private async Task<string> RequestTokenIfNeeded()
        {
            if (memoryCache.TryGetValue(cacheTokenKey, out string token))
            {
                return token;
            }

            TokenRequestResponse response;
            try
            {

                response = await HttpRequestHelper.SendPostRequestAsync<TokenRequestResponse>(
                    new RequestParameters
                    {
                        BaseAddress = configuration.BaseUrl,
                        Path = "/auth/oauth/token",
                        Auth = new AuthenticationHeaderValue("Basic",
                            HttpRequestHelper.ConvertToBase64(configuration.Username, configuration.Password))
                    }
                );
            }
            catch(Exception e)
            {
                logger.LogError("There was an error requesting an access token for the epc api: {}", e.Message);
                throw;
            }
            // We divide by 2 to avoid edge cases of sending requests on the exact expiration time
            var expiryTimeInSeconds = response.ExpiryTimeInSeconds / 2;
            token = response.Token;

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(expiryTimeInSeconds));

            memoryCache.Set(cacheTokenKey, token, cacheEntryOptions);
            return token;
        }
    }

    internal class TokenRequestResponse
    {
        [JsonProperty(PropertyName = "access_token")]
        public string Token { get; set; }
        [JsonProperty(PropertyName = "expires_in")]
        public int ExpiryTimeInSeconds { get; set; }
        [JsonProperty(PropertyName = "token_type")]
        public string TokenType { get; set; }
    }
}
