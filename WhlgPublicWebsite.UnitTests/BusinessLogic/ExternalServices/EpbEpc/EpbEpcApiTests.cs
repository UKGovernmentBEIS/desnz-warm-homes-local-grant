using FluentAssertions;
using NUnit.Framework;
using WhlgPublicWebsite.BusinessLogic.Models.Enums;
using WhlgPublicWebsite.BusinessLogic.ExternalServices.EpbEpc;
using Microsoft.Extensions.Options;
using RichardSzalay.MockHttp;
using WhlgPublicWebsite.BusinessLogic.ExternalServices.Common;
using System.Threading.Tasks;
using System;
using WhlgPublicWebsite.BusinessLogic.Models;
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
    public async Task EpcFromUprnAsync_CalledWithValidUprn_TranslatesResponseIntoEpcDetailsObject()
    {
        // Arrange
        mockHttpHandler.Expect("http://test.com/retrofit-funding/assessments")
            .WithHeaders("Authorization", "Bearer foobar")
            .Respond("application/json", @"{
  'data': {
    'assessment': {
      'address': {
        'addressLine1': '42 TRURO ROAD',
        'addressLine2': 'Upper Wellgood',
        'addressLine3': 'A Building',
        'addressLine4': 'A Place',
        'town': 'LIVERPOOL',
        'postcode': 'L15 9HW'
      },
      'uprn': '000038153332',
      'lodgementDate': '2017-04-08',
      'expiryDate': '2027-04-07',
      'currentBand': 'c',
      'propertyType': 'Detached house',
      'builtForm': 'Detached'
    }
  },
  'meta': {}
}"
        );

        var correctAssessment = new EpcDetails
        {
            AddressLine1 = "42 TRURO ROAD",
            AddressLine2 = "Upper Wellgood",
            AddressLine3 = "A Building",
            AddressLine4 = "A Place",
            AddressTown = "LIVERPOOL",
            AddressPostcode = "L15 9HW",
            LodgementDate = new DateTime(2017, 4, 8),
            ExpiryDate = new DateTime(2027, 4, 7),
            EpcRating = EpcRating.C,
            PropertyType = PropertyType.House,
            HouseType = HouseType.Detached,
            FlatType = null,
            BungalowType = null
        };

        // Act
        var assessment = await epcApi.EpcFromUprnAsync("000038153332");

        // Assert
        assessment.Should().BeEquivalentTo(correctAssessment);
    }
}
