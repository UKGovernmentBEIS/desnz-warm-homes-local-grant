using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace SeaPublicWebsite.ExternalServices.PostcodesIo
{
    public class PostcodesIoApi
    {
        private readonly ILogger<PostcodesIoApi> logger;
        
        public PostcodesIoApi(ILogger<PostcodesIoApi> logger)
        {
            this.logger = logger;
        }
        
        public async Task<bool> IsValidPostcode(string postcode)
        {
            if (string.IsNullOrWhiteSpace(postcode))
            {
                // We used to see quite a lot of error in the dependency logs saying "GET /postcodes//validate returned a 404"
                // This isn't wrong, but it makes the error logs a bit noisy
                // To make this a bit less noisy, we can reject any null / empty postcodes as being invalid
                return false;
            }

            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri("https://api.postcodes.io");

                    string path = $"/postcodes/{postcode}/validate";

                    HttpResponseMessage response = httpClient.GetAsync(path).Result;
                    string bodyString = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        var body = JsonConvert.DeserializeObject<PostcodesIoApiValidateResponse>(bodyString);
                        return body.result;
                    }
                    
                    // If postcodes.io has issues we don't want to block users from continuing
                    logger.LogError($"PostcodesIo returned an error. Code: {response.StatusCode} Content: {bodyString}");
                    return true;
                }
            }
            catch (Exception e)
            {
                // If postcodes.io has issues we don't want to block users from continuing
                logger.LogError($"Exception thrown communicating with postcodesIo: {e.Message}");
                return true;
            }
        }
    }
    
    internal class PostcodesIoApiValidateResponse
    {
        public bool result { get; set; }
    }
}