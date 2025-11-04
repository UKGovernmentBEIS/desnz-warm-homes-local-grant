using System.Net;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using WhlgPublicWebsite.BusinessLogic.ExternalServices.Common;
using WhlgPublicWebsite.BusinessLogic.Models;

namespace WhlgPublicWebsite.BusinessLogic.ExternalServices.EpbEpc;

/**
 * Documented at <see href="https://api-docs.epcregisters.net/#/Home%20Energy%20Retrofit%20Funding/"/>
 * There is a dropdown at the top left where you can select live/staging API. Select the staging API.
 * Credentials to use can be found in keeper. Select the retrofit-funding:assessment:fetch scope when authorizing.
 */
public class EpbEpcApi : IEpcApi
{
    private readonly EpbEpcConfiguration config;
    private readonly IMemoryCache memoryCache;
    private readonly ILogger logger;
    private readonly string cacheTokenKey = "EpbEpcToken";

    public EpbEpcApi(IOptions<EpbEpcConfiguration> options, IMemoryCache memoryCache, ILogger<EpbEpcApi> logger)
    {
        this.config = options.Value;
        this.memoryCache = memoryCache;
        this.logger = logger;
    }

    public async Task<EpcDetails> EpcFromUprnAsync(string uprn)
    {
        string token = null;
        try
        {
            token = await RequestTokenIfNeeded();
        }
        catch (Exception e)
        {
            logger.LogError("Unable to get EPB API Token: {}", e.Message);
            return null;
        }

        // This spec suggests UPRNs shouldn't need leading zeroes, but we need them for the EPC API
        // https://github.com/co-cddo/open-standards/issues/68
        var paddedUrn = uprn.PadLeft(12, '0');

        var parameters = new RequestParameters
        {
            BaseAddress = config.BaseUrl,
            Path = $"retrofit-funding/assessments?uprn={paddedUrn}",
            Auth = new AuthenticationHeaderValue("Bearer", token)
        };

        try
        {
            var response = await HttpRequestHelper.SendGetRequestAsync<EpbEpcDto>(parameters);
            return response.Data.Assessment.Parse();
        }
        catch (ApiException e) when (e.StatusCode == HttpStatusCode.NotFound)
        {
            logger.LogInformation("EPB EPC request could not find any results for UPRN {}", uprn);
            return null;
        }
        catch (Exception e)
        {
            logger.LogError("EPB EPC request failed: {}", e.Message);
            return null;
        }
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
                    BaseAddress = config.BaseUrl,
                    Path = "/auth/oauth/token",
                    Auth = new AuthenticationHeaderValue("Basic",
                        HttpRequestHelper.ConvertToBase64(config.Username, config.Password))
                }
            );
        }
        catch (Exception e)
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

    internal record class TokenRequestResponse
    {
        [JsonProperty(PropertyName = "access_token")]
        public string Token { get; set; }

        [JsonProperty(PropertyName = "expires_in")]
        public int ExpiryTimeInSeconds { get; set; }

        [JsonProperty(PropertyName = "token_type")]
        public string TokenType { get; set; }
    }
}