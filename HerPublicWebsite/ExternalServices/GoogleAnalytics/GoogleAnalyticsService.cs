using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using HerPublicWebsite.BusinessLogic.ExternalServices.Common;

namespace HerPublicWebsite.ExternalServices.GoogleAnalytics;

public class GoogleAnalyticsService
{
    public readonly GoogleAnalyticsConfiguration Configuration;
    private readonly ILogger<GoogleAnalyticsService> logger;

    // If these strings are ever updated you will need to update the Google Analytics custom events on the GA site to match.
    private const string EventNameBoilerQuestionViewed = "boiler_question_viewed";
    private const string EventNameQuestionnaireCompleted = "questionnaire_completed";
    private const string EventNameReferralGenerated = "referral_generated";
    
    public GoogleAnalyticsService(
        IOptions<GoogleAnalyticsConfiguration> options,
        ILogger<GoogleAnalyticsService> logger)
    {
        this.Configuration = options.Value;
        this.logger = logger;
    }
    
    public async Task SendBoilerQuestionViewedEventAsync(HttpRequest request)
    {
        await SendEventAsync(new GaRequestBody
        {
            ClientId = GetClientId(request),
            GaEvents = new List<GaEvent>
            {
                new()
                {
                    Name = EventNameBoilerQuestionViewed
                }
            }
        });
    }

    public async Task SendQuestionnaireCompletedEventAsync(HttpRequest request)
    {
        await SendEventAsync(new GaRequestBody
        {
            ClientId = GetClientId(request),
            GaEvents = new List<GaEvent>
            {
                new()
                {
                    Name = EventNameQuestionnaireCompleted
                }
            }
        });
    }
    
    public async Task SendReferralGeneratedEventAsync(HttpRequest request)
    {
        await SendEventAsync(new GaRequestBody
        {
            ClientId = GetClientId(request),
            GaEvents = new List<GaEvent>
            {
                new()
                {
                    Name = EventNameReferralGenerated
                }
            }
        });
    }
    
    private async Task SendEventAsync(GaRequestBody body)
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
    private string GetClientId(HttpRequest request)
    {
        return request.Cookies.TryGetValue(Configuration.CookieName, out var cookie)
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