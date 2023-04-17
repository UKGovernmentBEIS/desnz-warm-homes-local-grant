using HerPublicWebsite.BusinessLogic.Models.Enums;
using HerPublicWebsite.BusinessLogic.ExternalServices.Common;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace HerPublicWebsite.BusinessLogic.ExternalServices.EpbEpc
{

    public record struct EpcAddress
    {
        [JsonProperty("addressLine1")]
        public string AddressLine1 { get; set; }

        [JsonProperty("addressLine2")]
        public string AddressLine2 { get; set; }

        [JsonProperty("addressLine3")]
        public string AddressLine3 { get; set; }

        [JsonProperty("addressLine4")]
        public string AddressLine4 { get; set; }

        [JsonProperty("town")]
        public string Town { get; set; }

        [JsonProperty("postcode")]
        public string Postcode { get; set; }
    }

    public record struct EpcAssessment
    {
        [JsonProperty("address")]
        public EpcAddress Address { get; set; }

        [JsonProperty("uprn")]
        public string Uprn { get; set; }

        [JsonProperty("lodgementDate")]
        public DateTime LodgementDate { get; set; }

        [JsonProperty("currentBand")]
        public EpcRating CurrentBand { get; set; }
    }

    public record struct Data
    {
        [JsonProperty("assessment")]
        public EpcAssessment Assessment { get; set; }
    }

    public record struct Root
    {
        [JsonProperty("data", Required = Required.Always)]
        public Data Data { get; set; }
    }

    public class EpbEpcApi : IEpcApi
    {
        private readonly EpbEpcConfiguration config;

        public EpbEpcApi(IOptions<EpbEpcConfiguration> options)
        {
            config = options.Value;
        }

        public async Task<EpcAssessment> EpcFromUprn(string uprn)
        {
            var parameters = new RequestParameters {
                BaseAddress = config.BaseUrl,
                Path = $"/retrofit-funding/assessments?uprn={uprn}",
                Auth = new AuthenticationHeaderValue("Basic", config.Token)
            };

            var response = await HttpRequestHelper.SendGetRequestAsync<Root>(parameters);

            return response.Data.Assessment;
        }
    }
}
