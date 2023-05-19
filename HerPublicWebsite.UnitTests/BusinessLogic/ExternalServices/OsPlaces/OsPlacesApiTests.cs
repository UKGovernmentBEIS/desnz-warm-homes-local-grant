using FluentAssertions;
using NUnit.Framework;
using Microsoft.Extensions.Options;
using RichardSzalay.MockHttp;
using HerPublicWebsite.BusinessLogic.ExternalServices.Common;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using HerPublicWebsite.BusinessLogic.ExternalServices.OsPlaces;
using HerPublicWebsite.BusinessLogic.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Tests.BusinessLogic.ExternalServices.OsPlaces;

[TestFixture]
public class OsPlacesApiTests
{
    private OsPlacesApi underTest;
    private IMemoryCache memoryCache;
    private ILogger<OsPlacesApi> logger;
    private MockHttpMessageHandler mockHttpHandler;

    [SetUp]
    public void Setup()
    {
        var config = new OsPlacesConfiguration
        {
            Key = "testKey",
            BaseUrl = "http://test.com"
        };

        logger = new NullLogger<OsPlacesApi>();
        memoryCache = new MemoryCache(Options.Create(new MemoryCacheOptions()));
        memoryCache.Set("EpbEpcToken", "foobar");

        underTest = new OsPlacesApi(Options.Create(config), logger);

        mockHttpHandler = new MockHttpMessageHandler();
        HttpRequestHelper.handler = mockHttpHandler;
    }

    [Test]
    public async Task GetAddressesAsync_CalledWithValidPostcode_TranslatesResponseIntoAddresses()
    {
        // Arrange
        mockHttpHandler.Expect("http://test.com/search/places/v1/postcode?postcode=AB1%202CD")
            .WithHeaders("Key", "testKey")
            .Respond("application/json", DummyOsPlacesResponse);

        var correctAddresses = new List<Address>
        {
            new Address
            {
                AddressLine1 = "45 The Dene",
                AddressLine2 = null,
                Town = "Luton",
                Postcode = "AB1 2CD",
                County = null,
                Uprn = "123456789012",
                LocalCustodianCode = "1435"
            },
            new Address
            {
                AddressLine1 = "46 The Dene",
                AddressLine2 = null,
                Town = "Luton",
                Postcode = "AB1 2CD",
                County = null,
                Uprn = "123456789013",
                LocalCustodianCode = "1435"
            }
        };

        // Act
        var result = await underTest.GetAddressesAsync("AB1 2CD", "Non matching name");
        
        // Assert
        result.Should().BeEquivalentTo(correctAddresses);
    }
    
    [Test]
    public async Task GetAddressesAsync_CalledWithMatchingHouseNumber_OnlyReturnsMatch()
    {
        // Arrange
        mockHttpHandler.Expect("http://test.com/search/places/v1/postcode?postcode=AB1%202CD")
            .WithHeaders("Key", "testKey")
            .Respond("application/json", DummyOsPlacesResponse);

        // Act
        var result = await underTest.GetAddressesAsync("AB1 2CD", "45");
        
        // Assert
        result.Count.Should().Be(1);
        result.Single().AddressLine1.Should().Be("45 The Dene");
    }
    
    [Test]
    public async Task GetAddressesAsync_CalledWithoutMatchingHouseNumber_ReturnsAllResults()
    {
        // Arrange
        mockHttpHandler.Expect("http://test.com/search/places/v1/postcode?postcode=AB1%202CD")
            .WithHeaders("Key", "testKey")
            .Respond("application/json", DummyOsPlacesResponse);

        // Act
        var result = await underTest.GetAddressesAsync("AB1 2CD", "55");
        
        // Assert
        result.Count.Should().Be(2);
        result.Should().ContainSingle(a => a.AddressLine1 == "45 The Dene");
        result.Should().ContainSingle(a => a.AddressLine1 == "46 The Dene");
    }
    
    [Test]
    public async Task GetAddressesAsync_CalledWithMatchingHouseName_OnlyReturnsMatch()
    {
        // Arrange
        mockHttpHandler.Expect("http://test.com/search/places/v1/postcode?postcode=AB1%202CD")
            .WithHeaders("Key", "testKey")
            .Respond("application/json", DummyOsPlacesResponseWithNames);

        // Act
        var result = await underTest.GetAddressesAsync("AB1 2CD", "Big House");
        
        // Assert
        result.Count.Should().Be(1);
        result.Single().AddressLine1.Should().Be("Big House The Dene");
    }
    
    [Test]
    public async Task GetAddressesAsync_CalledWithInvalidPostcode_ReturnsEmptyList()
    {
        // Act
        var result = await underTest.GetAddressesAsync("Not a postcode", "Big House");
        
        // Assert
        result.Count.Should().Be(0);
    }

    // This is a real response from the API that has had it's data replaced with dummy values
    private const string DummyOsPlacesResponse = @"{
  ""header"" : {
    ""uri"" : ""https://api.os.uk/search/places/v1/postcode?postcode=AB1%202CD"",
    ""query"" : ""postcode=AB1 2CD"",
    ""offset"" : 0,
    ""totalresults"" : 10,
    ""format"" : ""JSON"",
    ""dataset"" : ""DPA"",
    ""lr"" : ""EN,CY"",
    ""maxresults"" : 100,
    ""epoch"" : ""101"",
    ""lastupdate"" : ""2023-05-18"",
    ""output_srs"" : ""EPSG:27700""
  },
  ""results"" : [ {
    ""DPA"" : {
      ""UPRN"" : ""123456789012"",
      ""UDPRN"" : ""12345671"",
      ""ADDRESS"" : ""45, THE DENE, LUTON, AB1 2CD"",
      ""BUILDING_NUMBER"" : ""45"",
      ""THOROUGHFARE_NAME"" : ""THE DENE"",
      ""POST_TOWN"" : ""LUTON"",
      ""POSTCODE"" : ""AB1 2CD"",
      ""RPC"" : ""1"",
      ""X_COORDINATE"" : 123456.78,
      ""Y_COORDINATE"" : 123456.78,
      ""STATUS"" : ""APPROVED"",
      ""LOGICAL_STATUS_CODE"" : ""1"",
      ""CLASSIFICATION_CODE"" : ""RD04"",
      ""CLASSIFICATION_CODE_DESCRIPTION"" : ""Terraced"",
      ""LOCAL_CUSTODIAN_CODE"" : 1435,
      ""LOCAL_CUSTODIAN_CODE_DESCRIPTION"" : ""WEALDEN"",
      ""COUNTRY_CODE"" : ""E"",
      ""COUNTRY_CODE_DESCRIPTION"" : ""This record is within England"",
      ""POSTAL_ADDRESS_CODE"" : ""D"",
      ""POSTAL_ADDRESS_CODE_DESCRIPTION"" : ""A record which is linked to PAF"",
      ""BLPU_STATE_CODE"" : ""2"",
      ""BLPU_STATE_CODE_DESCRIPTION"" : ""In use"",
      ""TOPOGRAPHY_LAYER_TOID"" : ""osgb1000000012345"",
      ""LAST_UPDATE_DATE"" : ""24/04/2016"",
      ""ENTRY_DATE"" : ""12/11/2001"",
      ""BLPU_STATE_DATE"" : ""07/09/2007"",
      ""LANGUAGE"" : ""EN"",
      ""MATCH"" : 1.0,
      ""MATCH_DESCRIPTION"" : ""EXACT"",
      ""DELIVERY_POINT_SUFFIX"" : ""2A""
    }
  }, {
    ""DPA"" : {
      ""UPRN"" : ""123456789013"",
      ""UDPRN"" : ""12345672"",
      ""ADDRESS"" : ""46, THE DENE, LUTON, AB1 2CD"",
      ""BUILDING_NUMBER"" : ""46"",
      ""THOROUGHFARE_NAME"" : ""THE DENE"",
      ""POST_TOWN"" : ""LUTON"",
      ""POSTCODE"" : ""AB1 2CD"",
      ""RPC"" : ""1"",
      ""X_COORDINATE"" : 123457.78,
      ""Y_COORDINATE"" : 123457.78,
      ""STATUS"" : ""APPROVED"",
      ""LOGICAL_STATUS_CODE"" : ""1"",
      ""CLASSIFICATION_CODE"" : ""RD04"",
      ""CLASSIFICATION_CODE_DESCRIPTION"" : ""Terraced"",
      ""LOCAL_CUSTODIAN_CODE"" : 1435,
      ""LOCAL_CUSTODIAN_CODE_DESCRIPTION"" : ""WEALDEN"",
      ""COUNTRY_CODE"" : ""E"",
      ""COUNTRY_CODE_DESCRIPTION"" : ""This record is within England"",
      ""POSTAL_ADDRESS_CODE"" : ""D"",
      ""POSTAL_ADDRESS_CODE_DESCRIPTION"" : ""A record which is linked to PAF"",
      ""BLPU_STATE_CODE"" : ""2"",
      ""BLPU_STATE_CODE_DESCRIPTION"" : ""In use"",
      ""TOPOGRAPHY_LAYER_TOID"" : ""osgb1000000012346"",
      ""LAST_UPDATE_DATE"" : ""24/04/2016"",
      ""ENTRY_DATE"" : ""12/11/2001"",
      ""BLPU_STATE_DATE"" : ""07/09/2007"",
      ""LANGUAGE"" : ""EN"",
      ""MATCH"" : 1.0,
      ""MATCH_DESCRIPTION"" : ""EXACT"",
      ""DELIVERY_POINT_SUFFIX"" : ""2B""
    }
  } ]
}";
    
    private const string DummyOsPlacesResponseWithNames = @"{
  ""header"" : {
    ""uri"" : ""https://api.os.uk/search/places/v1/postcode?postcode=AB1%202CD"",
    ""query"" : ""postcode=AB1 2CD"",
    ""offset"" : 0,
    ""totalresults"" : 10,
    ""format"" : ""JSON"",
    ""dataset"" : ""DPA"",
    ""lr"" : ""EN,CY"",
    ""maxresults"" : 100,
    ""epoch"" : ""101"",
    ""lastupdate"" : ""2023-05-18"",
    ""output_srs"" : ""EPSG:27700""
  },
  ""results"" : [ {
    ""DPA"" : {
      ""UPRN"" : ""123456789012"",
      ""UDPRN"" : ""12345671"",
      ""ADDRESS"" : ""BIG HOUSE, THE DENE, LUTON, AB1 2CD"",
      ""BUILDING_NAME"" : ""BIG HOUSE"",
      ""THOROUGHFARE_NAME"" : ""THE DENE"",
      ""POST_TOWN"" : ""LUTON"",
      ""POSTCODE"" : ""AB1 2CD"",
      ""RPC"" : ""1"",
      ""X_COORDINATE"" : 123456.78,
      ""Y_COORDINATE"" : 123456.78,
      ""STATUS"" : ""APPROVED"",
      ""LOGICAL_STATUS_CODE"" : ""1"",
      ""CLASSIFICATION_CODE"" : ""RD04"",
      ""CLASSIFICATION_CODE_DESCRIPTION"" : ""Terraced"",
      ""LOCAL_CUSTODIAN_CODE"" : 1435,
      ""LOCAL_CUSTODIAN_CODE_DESCRIPTION"" : ""WEALDEN"",
      ""COUNTRY_CODE"" : ""E"",
      ""COUNTRY_CODE_DESCRIPTION"" : ""This record is within England"",
      ""POSTAL_ADDRESS_CODE"" : ""D"",
      ""POSTAL_ADDRESS_CODE_DESCRIPTION"" : ""A record which is linked to PAF"",
      ""BLPU_STATE_CODE"" : ""2"",
      ""BLPU_STATE_CODE_DESCRIPTION"" : ""In use"",
      ""TOPOGRAPHY_LAYER_TOID"" : ""osgb1000000012345"",
      ""LAST_UPDATE_DATE"" : ""24/04/2016"",
      ""ENTRY_DATE"" : ""12/11/2001"",
      ""BLPU_STATE_DATE"" : ""07/09/2007"",
      ""LANGUAGE"" : ""EN"",
      ""MATCH"" : 1.0,
      ""MATCH_DESCRIPTION"" : ""EXACT"",
      ""DELIVERY_POINT_SUFFIX"" : ""2A""
    }
  }, {
    ""DPA"" : {
      ""UPRN"" : ""123456789013"",
      ""UDPRN"" : ""12345672"",
      ""ADDRESS"" : ""SMALL HOUSE, THE DENE, LUTON, AB1 2CD"",
      ""BUILDING_NAME"" : ""SMALL HOUSE"",
      ""THOROUGHFARE_NAME"" : ""THE DENE"",
      ""POST_TOWN"" : ""LUTON"",
      ""POSTCODE"" : ""AB1 2CD"",
      ""RPC"" : ""1"",
      ""X_COORDINATE"" : 123457.78,
      ""Y_COORDINATE"" : 123457.78,
      ""STATUS"" : ""APPROVED"",
      ""LOGICAL_STATUS_CODE"" : ""1"",
      ""CLASSIFICATION_CODE"" : ""RD04"",
      ""CLASSIFICATION_CODE_DESCRIPTION"" : ""Terraced"",
      ""LOCAL_CUSTODIAN_CODE"" : 1435,
      ""LOCAL_CUSTODIAN_CODE_DESCRIPTION"" : ""WEALDEN"",
      ""COUNTRY_CODE"" : ""E"",
      ""COUNTRY_CODE_DESCRIPTION"" : ""This record is within England"",
      ""POSTAL_ADDRESS_CODE"" : ""D"",
      ""POSTAL_ADDRESS_CODE_DESCRIPTION"" : ""A record which is linked to PAF"",
      ""BLPU_STATE_CODE"" : ""2"",
      ""BLPU_STATE_CODE_DESCRIPTION"" : ""In use"",
      ""TOPOGRAPHY_LAYER_TOID"" : ""osgb1000000012346"",
      ""LAST_UPDATE_DATE"" : ""24/04/2016"",
      ""ENTRY_DATE"" : ""12/11/2001"",
      ""BLPU_STATE_DATE"" : ""07/09/2007"",
      ""LANGUAGE"" : ""EN"",
      ""MATCH"" : 1.0,
      ""MATCH_DESCRIPTION"" : ""EXACT"",
      ""DELIVERY_POINT_SUFFIX"" : ""2B""
    }
  } ]
}";
}
