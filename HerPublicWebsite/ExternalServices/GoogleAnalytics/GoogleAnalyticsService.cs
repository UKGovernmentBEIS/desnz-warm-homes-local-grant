using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using HerPublicWebsite.BusinessLogic.ExternalServices.Common;
using HerPublicWebsite.Services.Cookies;

namespace HerPublicWebsite.ExternalServices.GoogleAnalytics;

public class GoogleAnalyticsService
{
    public readonly GoogleAnalyticsConfiguration Configuration;
    private readonly CookieService cookieService;
    private readonly ILogger<GoogleAnalyticsService> logger;
    
    public GoogleAnalyticsService(
        IOptions<GoogleAnalyticsConfiguration> options,
        CookieService cookieService,
        ILogger<GoogleAnalyticsService> logger)
    {
        this.Configuration = options.Value;
        this.cookieService = cookieService;
        this.logger = logger;
    }
    
    public async Task SendEvent(GaRequestBody body)
    {
        try
        {
            await HttpRequestHelper.SendPostRequestAsync<string>(new RequestParameters
            {
                BaseAddress = Configuration.BaseUrl,
                Path = $"/mp/collect?api_secret={Configuration.ApiSecret}&measurement_id={Configuration.MeasurementId}",
                Body = new StringContent(JsonConvert.SerializeObject(body))
            });
        }
        catch (Exception e)
        {
            logger.LogError("There was an error sending an event to google analytics: {}", e.Message);
        }
    }
    
    // Cookie format: GAx.y.zzzzzzzzz.tttttttttt.
    // The z section is the client id
    // If we can't find the _ga cookie, return a new id
    public string GetClientId(HttpRequest request)
    {
        return cookieService.TryGetCookie<string>(request, Configuration.CookieName, out var cookie) 
            ? cookie.Split('.')[2] 
            : Guid.NewGuid().ToString();
    }
}

public class GaRequestBody
{
    [JsonProperty(PropertyName = "client_id")]
    public string ClientId { get; set; }
    
    [JsonProperty(PropertyName = "events")]
    public List<GaEvent> GaEvents { get; set; }
}

public class GaEvent
{
    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }
    
    [JsonProperty(PropertyName = "params")]
    public Dictionary<string, object> Parameters { get; set; }
}