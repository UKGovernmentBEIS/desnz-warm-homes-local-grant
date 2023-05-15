using FluentAssertions;
using NUnit.Framework;
using HerPublicWebsite.BusinessLogic.Models.Enums;
using HerPublicWebsite.BusinessLogic.ExternalServices.EpbEpc;
using Microsoft.Extensions.Options;
using RichardSzalay.MockHttp;
using HerPublicWebsite.BusinessLogic.ExternalServices.Common;
using System.Threading.Tasks;
using System;
using HerPublicWebsite.BusinessLogic.Models;
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

    [Test]
    public async Task GetEpcFromUpn()
    {
        // Arrange
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
              'lodgementDate': '2020-02-27',
              'expiryDate': '2030-02-27',
              'currentBand': 'D',
              'propertyType': 'Mid-floor flat',
              'builtForm': 'Flat'
            }
          }
        }");
        
        var correctAssessment = new EpcDetails
        {
            AddressLine1 = "22 Acacia Avenue",
            AddressLine2 = "Upper Wellgood",
            AddressLine3 = "A Building",
            AddressLine4 = "A Place",
            AddressTown = "Fulchester",
            AddressPostcode = "FL23 4JA",
            LodgementDate = new DateTime(2020, 2, 27),
            ExpiryDate = new DateTime(2030, 2, 27),
            EpcRating = EpcRating.D,
            PropertyType = PropertyType.ApartmentFlatOrMaisonette,
            HouseType = null,
            FlatType = FlatType.MiddleFloor,
            BungalowType = null
        };

        // Act
        var assessment = await epcApi.EpcFromUprnAsync("001234567890");
        
        // Assert
        assessment.Should().BeEquivalentTo(correctAssessment);
    }
}
