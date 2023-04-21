using FluentAssertions;
using NUnit.Framework;
using HerPublicWebsite.BusinessLogic.Models.Enums;
using HerPublicWebsite.BusinessLogic.ExternalServices.EpbEpc;
using Microsoft.Extensions.Options;
using RichardSzalay.MockHttp;
using HerPublicWebsite.BusinessLogic.ExternalServices.Common;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Tests.BusinessLogic.ExternalServices.EpbEpc;

[TestFixture]
public class EpbEpcApiTests
{
    private IEpcApi epcApi;
    private IMemoryCache memoryCache;
    private ILogger<EpbEpcApi> logger;
    private MockHttpMessageHandler mockHttpHandler;

    [SetUp]
    public void Setup()
    {
        var config = new EpbEpcConfiguration
        {
            Username = "foo",
            Password = "bar",
            BaseUrl = "http://test.com"
        };

        logger = new NullLogger<EpbEpcApi>();
        memoryCache = new MemoryCache(Options.Create(new MemoryCacheOptions()));
        memoryCache.Set("EpbEpcToken", "foobar");

        epcApi = new EpbEpcApi(Options.Create(config), memoryCache, logger);

        mockHttpHandler = new MockHttpMessageHandler();
        HttpRequestHelper.handler = mockHttpHandler;
    }

    private readonly EpcAssessmentDto correctAssessment = new()
    {
        Address = new EpcAddressDto
        {
            AddressLine1 = "22 Acacia Avenue",
            AddressLine2 = "Upper Wellgood",
            AddressLine3 = "A Building",
            AddressLine4 = "A Place",
            Town = "Fulchester",
            Postcode = "FL23 4JA"
        },
        Uprn = "001234567890",
        LodgementDate = new DateTime(2020, 2, 29),
        CurrentBand = EpcRating.D
    };

    [Test]
    public async Task GetEpcFromUpn()
    {
        mockHttpHandler.Expect("http://test.com/retrofit-funding/assessments")
            .WithHeaders("Authorization", "Bearer foobar")
            .Respond("application/json", @"{
          'data': {
            'assessment': {
              'address': {
                'addressLine1': '22 Acacia Avenue',
                'addressLine2': 'Upper Wellgood',
                'addressLine3': 'A Building',
                'addressLine4': 'A Place',
                'town': 'Fulchester',
                'postcode': 'FL23 4JA'
              },
              'uprn': '001234567890',
              'lodgementDate': '2020-02-29',
              'currentBand': 'D'
            }
          }
        }");

        var assessment = await epcApi.EpcFromUprn("001234567890");
        assessment.Should().Be(correctAssessment);
    }
}
