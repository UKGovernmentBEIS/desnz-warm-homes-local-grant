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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Tests.BusinessLogic.ExternalServices.OsPlaces;

[TestFixture]
public class OsPlacesApiTests
{
    private OsPlacesApi underTest;
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
                AddressLine1 = "45, The Dene",
                AddressLine2 = "",
                Town = "Luton",
                Postcode = "AB1 2CD",
                County = null,
                Uprn = "123456789012",
                LocalCustodianCode = "1435"
            },
            new Address
            {
                AddressLine1 = "46, The Dene",
                AddressLine2 = "",
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
        result.Single().AddressLine1.Should().Be("45, The Dene");
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
        result.Should().ContainSingle(a => a.AddressLine1 == "45, The Dene");
        result.Should().ContainSingle(a => a.AddressLine1 == "46, The Dene");
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
        result.Single().AddressLine1.Should().Be("Big House, The Dene");
    }
    
    [Test]
    public async Task GetAddressesAsync_CalledWithMatchingFlat_OnlyReturnsMatch()
    {
        // Arrange
        mockHttpHandler.Expect("http://test.com/search/places/v1/postcode?postcode=AB1%202CD")
            .WithHeaders("Key", "testKey")
            .Respond("application/json", DummyOsPlacesResponseWithFlat);

        // Act
        var result = await underTest.GetAddressesAsync("AB1 2CD", "Flat 1");
        
        // Assert
        result.Count.Should().Be(1);
        result.Single().AddressLine1.Should().Be("Flat 1, Big House, The Dene");
    }
    
    [Test]
    public async Task GetAddressesAsync_CalledWithInvalidPostcode_ReturnsEmptyList()
    {
        // Act
        var result = await underTest.GetAddressesAsync("Not a postcode", "Big House");
        
        // Assert
        result.Count.Should().Be(0);
    }
        
    [Test]
    public async Task GetAddressesAsync_CalledWithEmptyPostcode_ReturnsEmptyList()
    {
        // Arrange
        mockHttpHandler.Expect("http://test.com/search/places/v1/postcode?postcode=AB1%202CD")
            .WithHeaders("Key", "testKey")
            .Respond("application/json", DummyOsPlacesResponseWithNoResults);
        
        // Act
        var result = await underTest.GetAddressesAsync("AB1 2CD", "Big House");
        
        // Assert
        result.Count.Should().Be(0);
    }
    
    [Test]
    public async Task GetAddressesAsync_WhenThereAreMoreMatchesThanCanBeReturned_CallsOsPlacesAgain()
    {
        // Arrange
        mockHttpHandler.Expect("http://test.com/search/places/v1/postcode?postcode=AB1%202CD")
            .WithHeaders("Key", "testKey")
            .Respond("application/json", DummyOsPlacesResponseWithMoreResults);
        mockHttpHandler.Expect("http://test.com/search/places/v1/postcode?postcode=AB1%202CD&offset=100")
            .WithHeaders("Key", "testKey")
            .Respond("application/json", DummyOsPlacesResponseWithMoreResultsPart2);
        
        // Act
        var result = await underTest.GetAddressesAsync("AB1 2CD", "No match");
        
        // Assert
        result.Count.Should().Be(2);
        mockHttpHandler.VerifyNoOutstandingExpectation();
    }
    
    [Test]
    public async Task GetAddressesAsync_WhenReceivesLpiResults_TranslatesResponseIntoAddresses()
    {
        // Arrange
        mockHttpHandler.Expect("http://test.com/search/places/v1/postcode?postcode=AB1%202CD")
            .WithHeaders("Key", "testKey")
            .Respond("application/json", DummyOsPlacesResponseWithOnlyLpiResults);

        var correctAddresses = new List<Address>
        {
            new()
            {
                AddressLine1 = "Cafe, Example Studios",
                AddressLine2 = "1–10, Example Road",
                Town = "London",
                Postcode = "AB1 2CD",
                County = null,
                Uprn = "12345678901",
                LocalCustodianCode = "5210"
            },
            new()
            {
                AddressLine1 = "Car Parking Spaces, Example Studios",
                AddressLine2 = "1–10, Example Road",
                Town = "London",
                Postcode = "AB1 2CD",
                County = null,
                Uprn = "12345678902",
                LocalCustodianCode = "5210"
            },
            new()
            {
                AddressLine1 = "Example Organisation, Premises In Unit 1, Example Studios",
                AddressLine2 = "1–10, Example Road",
                Town = "London",
                Postcode = "AB1 2CD",
                County = null,
                Uprn = "12345678903",
                LocalCustodianCode = "5210"
            },
            new()
            {
                AddressLine1 = "Premises In Unit 2, Example Studios",
                AddressLine2 = "1–10, Example Road",
                Town = "London",
                Postcode = "AB1 2CD",
                County = null,
                Uprn = "12345678904",
                LocalCustodianCode = "5210"
            },
            new()
            {
                AddressLine1 = "Premises In Unit 3, Example Studios",
                AddressLine2 = "1–10, Example Road",
                Town = "London",
                Postcode = "AB1 2CD",
                County = null,
                Uprn = "12345678905",
                LocalCustodianCode = "5210"
            },
        };

        // Act
        var result = await underTest.GetAddressesAsync("AB1 2CD", "Non matching name");
        
        // Assert
        result.Should().BeEquivalentTo(correctAddresses);
    }
    
    [Test]
    public async Task GetAddressesAsync_WhenReceivesDpaAndLpiResults_CorrelatesTheResultsBasedOnUprn()
    {
        // Arrange
        mockHttpHandler.Expect("http://test.com/search/places/v1/postcode?postcode=AB1%202CD")
            .WithHeaders("Key", "testKey")
            .Respond("application/json", DummyOsPlacesResponseWithDpaAndLpiResults);

        var correctAddresses = new List<Address>
        {
            new()
            {
                AddressLine1 = "Example House, Example Road",
                AddressLine2 = "",
                Town = "London",
                Postcode = "AB1 2CD",
                County = null,
                Uprn = "12345671",
                LocalCustodianCode = "5150"
            },
            new()
            {
                AddressLine1 = "4, Example House",
                AddressLine2 = "Example Road",
                Town = "London",
                Postcode = "AB1 2CD",
                County = null,
                Uprn = "12345673",
                LocalCustodianCode = "5150"
            },
            new()
            {
                AddressLine1 = "3, Example House",
                AddressLine2 = "Example Road",
                Town = "London",
                Postcode = "AB1 2CD",
                County = null,
                Uprn = "12345674",
                LocalCustodianCode = "5150"
            },
            new()
            {
                AddressLine1 = "2, Example House",
                AddressLine2 = "Example Road",
                Town = "London",
                Postcode = "AB1 2CD",
                County = null,
                Uprn = "12345675",
                LocalCustodianCode = "5150"
            },
            new()
            {
                AddressLine1 = "1, Example House",
                AddressLine2 = "Example Road",
                Town = "London",
                Postcode = "AB1 2CD",
                County = null,
                Uprn = "12345676",
                LocalCustodianCode = "5150"
            },
        };

        // Act
        var result = await underTest.GetAddressesAsync("AB1 2CD", "Non matching name");
        
        // Assert
        result.Should().BeEquivalentTo(correctAddresses);
    }
    
    [Test]
    public async Task GetAddressesAsync_WhenReceivesNonCurrentResidentialLpiResults_RemovesAddresses()
    {
        // Arrange
        mockHttpHandler.Expect("http://test.com/search/places/v1/postcode?postcode=AB1%202CD")
            .WithHeaders("Key", "testKey")
            .Respond("application/json", DummyOsPlacesResponseWithNonResidentialResults);

        var correctAddresses = new List<Address>
        {
            new()
            {
                AddressLine1 = "Premises In Unit 2, Example Studios",
                AddressLine2 = "1–10, Example Road",
                Town = "London",
                Postcode = "AB1 2CD",
                County = null,
                Uprn = "12345678904",
                LocalCustodianCode = "5210"
            },
        };

        // Act
        var result = await underTest.GetAddressesAsync("AB1 2CD", "Non matching name");
        
        // Assert
        result.Should().BeEquivalentTo(correctAddresses);
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
    
    
    private const string DummyOsPlacesResponseWithFlat = @"{
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
      ""ADDRESS"" : ""FLAT 1, BIG HOUSE, THE DENE, LUTON, AB1 2CD"",
      ""SUB_BUILDING_NAME"" : ""FLAT 1"",
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

    private const string DummyOsPlacesResponseWithNoResults = @"{
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
  }
}";
    
    private const string DummyOsPlacesResponseWithMoreResults = @"{
  ""header"" : {
    ""uri"" : ""https://api.os.uk/search/places/v1/postcode?postcode=AB1%202CD"",
    ""query"" : ""postcode=AB1 2CD"",
    ""offset"" : 0,
    ""totalresults"" : 120,
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
      ""ADDRESS"" : ""FLAT 1, BIG HOUSE, THE DENE, LUTON, AB1 2CD"",
      ""SUB_BUILDING_NAME"" : ""FLAT 1"",
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
  } ]
}";
    
    private const string DummyOsPlacesResponseWithMoreResultsPart2 = @"{
  ""header"" : {
    ""uri"" : ""https://api.os.uk/search/places/v1/postcode?postcode=AB1%202CD"",
    ""query"" : ""postcode=AB1 2CD"",
    ""offset"" : 0,
    ""totalresults"" : 120,
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
    
    private const string DummyOsPlacesResponseWithOnlyLpiResults = @"{
    ""header"": {
        ""uri"" : ""https://api.os.uk/search/places/v1/postcode?postcode=AB1%202CD"",
        ""query"" : ""postcode=AB1 2CD"",
        ""offset"" : 0,
        ""totalresults"" : 5,
        ""format"" : ""JSON"",
        ""dataset"" : ""LPI"",
        ""lr"" : ""EN,CY"",
        ""maxresults"" : 100,
        ""epoch"" : ""101"",
        ""lastupdate"" : ""2023-05-18"",
        ""output_srs"" : ""EPSG:27700""
      },
    ""results"": [
        {
            ""LPI"": {
                ""UPRN"": ""12345678901"",
                ""ADDRESS"": ""CAFE, EXAMPLE STUDIOS, 1-10, EXAMPLE ROAD, LONDON, CAMDEN, AB1 2CD"",
                ""USRN"": ""9876543"",
                ""LPI_KEY"": ""5150L000099999"",
                ""SAO_TEXT"": ""CAFE"",
                ""PAO_START_NUMBER"": ""1"",
                ""PAO_END_NUMBER"": ""10"",
                ""PAO_TEXT"": ""EXAMPLE STUDIOS"",
                ""STREET_DESCRIPTION"": ""EXAMPLE ROAD"",
                ""TOWN_NAME"": ""LONDON"",
                ""ADMINISTRATIVE_AREA"": ""CAMDEN"",
                ""POSTCODE_LOCATOR"": ""AB1 2CD"",
                ""RPC"": ""2"",
                ""X_COORDINATE"": 1234567.78,
                ""Y_COORDINATE"": 1234567.78,
                ""STATUS"": ""APPROVED"",
                ""LOGICAL_STATUS_CODE"": ""1"",
                ""CLASSIFICATION_CODE"": ""RD07"",
                ""CLASSIFICATION_CODE_DESCRIPTION"": ""Restaurant / Cafeteria"",
                ""LOCAL_CUSTODIAN_CODE"": 5210,
                ""LOCAL_CUSTODIAN_CODE_DESCRIPTION"": ""CAMDEN"",
                ""COUNTRY_CODE"": ""E"",
                ""COUNTRY_CODE_DESCRIPTION"": ""This record is within England"",
                ""POSTAL_ADDRESS_CODE"": ""C"",
                ""POSTAL_ADDRESS_CODE_DESCRIPTION"": ""A record which is postal and has a parent record which is linked to PAF"",
                ""BLPU_STATE_CODE"": ""2"",
                ""BLPU_STATE_CODE_DESCRIPTION"": ""In use"",
                ""TOPOGRAPHY_LAYER_TOID"": ""osgb1000000000000"",
                ""PARENT_UPRN"": ""9090909"",
                ""LAST_UPDATE_DATE"": ""25/03/2022"",
                ""ENTRY_DATE"": ""08/10/2002"",
                ""BLPU_STATE_DATE"": ""08/10/2002"",
                ""STREET_STATE_CODE"": ""2"",
                ""STREET_STATE_CODE_DESCRIPTION"": ""Open"",
                ""STREET_CLASSIFICATION_CODE"": ""8"",
                ""STREET_CLASSIFICATION_CODE_DESCRIPTION"": ""All vehicles"",
                ""LPI_LOGICAL_STATUS_CODE"": ""1"",
                ""LPI_LOGICAL_STATUS_CODE_DESCRIPTION"": ""APPROVED"",
                ""LANGUAGE"": ""EN"",
                ""MATCH"": 1.0,
                ""MATCH_DESCRIPTION"": ""EXACT""
            }
        },
        {
            ""LPI"": {
                ""UPRN"": ""12345678902"",
                ""ADDRESS"": ""CAR PARKING SPACES, EXAMPLE STUDIOS, 1-10, EXAMPLE ROAD, LONDON, CAMDEN, AB1 2CD"",
                ""USRN"": ""9876543"",
                ""LPI_KEY"": ""5150L000099999"",
                ""SAO_TEXT"": ""CAR PARKING SPACES"",
                ""PAO_START_NUMBER"": ""1"",
                ""PAO_END_NUMBER"": ""10"",
                ""PAO_TEXT"": ""EXAMPLE STUDIOS"",
                ""STREET_DESCRIPTION"": ""EXAMPLE ROAD"",
                ""TOWN_NAME"": ""LONDON"",
                ""ADMINISTRATIVE_AREA"": ""CAMDEN"",
                ""POSTCODE_LOCATOR"": ""AB1 2CD"",
                ""RPC"": ""2"",
                ""X_COORDINATE"": 1234567.78,
                ""Y_COORDINATE"": 1234567.78,
                ""STATUS"": ""APPROVED"",
                ""LOGICAL_STATUS_CODE"": ""1"",
                ""CLASSIFICATION_CODE"": ""RD01"",
                ""CLASSIFICATION_CODE_DESCRIPTION"": ""Commercial"",
                ""LOCAL_CUSTODIAN_CODE"": 5210,
                ""LOCAL_CUSTODIAN_CODE_DESCRIPTION"": ""CAMDEN"",
                ""COUNTRY_CODE"": ""E"",
                ""COUNTRY_CODE_DESCRIPTION"": ""This record is within England"",
                ""POSTAL_ADDRESS_CODE"": ""C"",
                ""POSTAL_ADDRESS_CODE_DESCRIPTION"": ""A record which is postal and has a parent record which is linked to PAF"",
                ""BLPU_STATE_CODE"": ""2"",
                ""BLPU_STATE_CODE_DESCRIPTION"": ""In use"",
                ""TOPOGRAPHY_LAYER_TOID"": ""osgb1000000000000"",
                ""PARENT_UPRN"": ""9090909"",
                ""LAST_UPDATE_DATE"": ""25/03/2022"",
                ""ENTRY_DATE"": ""08/10/2002"",
                ""BLPU_STATE_DATE"": ""08/10/2002"",
                ""STREET_STATE_CODE"": ""2"",
                ""STREET_STATE_CODE_DESCRIPTION"": ""Open"",
                ""STREET_CLASSIFICATION_CODE"": ""8"",
                ""STREET_CLASSIFICATION_CODE_DESCRIPTION"": ""All vehicles"",
                ""LPI_LOGICAL_STATUS_CODE"": ""1"",
                ""LPI_LOGICAL_STATUS_CODE_DESCRIPTION"": ""APPROVED"",
                ""LANGUAGE"": ""EN"",
                ""MATCH"": 1.0,
                ""MATCH_DESCRIPTION"": ""EXACT""
            }
        },
        {
            ""LPI"": {
                ""UPRN"": ""12345678903"",
                ""ADDRESS"": ""EXAMPLE ORGANISATION, PREMISES IN UNIT 1, EXAMPLE STUDIOS, 1-10, EXAMPLE ROAD, LONDON, CAMDEN, AB1 2CD"",
                ""USRN"": ""9876543"",
                ""LPI_KEY"": ""5150L000099999"",
                ""ORGANISATION"": ""EXAMPLE ORGANISATION"",
                ""SAO_TEXT"": ""PREMISES IN UNIT 1"",
                ""PAO_START_NUMBER"": ""1"",
                ""PAO_END_NUMBER"": ""10"",
                ""PAO_TEXT"": ""EXAMPLE STUDIOS"",
                ""STREET_DESCRIPTION"": ""EXAMPLE ROAD"",
                ""TOWN_NAME"": ""LONDON"",
                ""ADMINISTRATIVE_AREA"": ""CAMDEN"",
                ""POSTCODE_LOCATOR"": ""AB1 2CD"",
                ""RPC"": ""2"",
                ""X_COORDINATE"": 1234567.78,
                ""Y_COORDINATE"": 1234567.78,
                ""STATUS"": ""APPROVED"",
                ""LOGICAL_STATUS_CODE"": ""1"",
                ""CLASSIFICATION_CODE"": ""RD06"",
                ""CLASSIFICATION_CODE_DESCRIPTION"": ""Indoor / Outdoor Leisure / Sporting Activity / Centre"",
                ""LOCAL_CUSTODIAN_CODE"": 5210,
                ""LOCAL_CUSTODIAN_CODE_DESCRIPTION"": ""CAMDEN"",
                ""COUNTRY_CODE"": ""E"",
                ""COUNTRY_CODE_DESCRIPTION"": ""This record is within England"",
                ""POSTAL_ADDRESS_CODE"": ""D"",
                ""POSTAL_ADDRESS_CODE_DESCRIPTION"": ""A record which is linked to PAF"",
                ""BLPU_STATE_CODE"": ""2"",
                ""BLPU_STATE_CODE_DESCRIPTION"": ""In use"",
                ""TOPOGRAPHY_LAYER_TOID"": ""osgb1000000000000"",
                ""PARENT_UPRN"": ""9090909"",
                ""LAST_UPDATE_DATE"": ""25/03/2022"",
                ""ENTRY_DATE"": ""08/10/2002"",
                ""BLPU_STATE_DATE"": ""08/10/2002"",
                ""STREET_STATE_CODE"": ""2"",
                ""STREET_STATE_CODE_DESCRIPTION"": ""Open"",
                ""STREET_CLASSIFICATION_CODE"": ""8"",
                ""STREET_CLASSIFICATION_CODE_DESCRIPTION"": ""All vehicles"",
                ""LPI_LOGICAL_STATUS_CODE"": ""1"",
                ""LPI_LOGICAL_STATUS_CODE_DESCRIPTION"": ""APPROVED"",
                ""LANGUAGE"": ""EN"",
                ""MATCH"": 1.0,
                ""MATCH_DESCRIPTION"": ""EXACT""
            }
        },
        {
            ""LPI"": {
                ""UPRN"": ""12345678904"",
                ""ADDRESS"": ""PREMISES IN UNIT 2, EXAMPLE STUDIOS, 1-10, EXAMPLE ROAD, LONDON, CAMDEN, AB1 2CD"",
                ""USRN"": ""9876543"",
                ""LPI_KEY"": ""5150L000099999"",
                ""SAO_TEXT"": ""PREMISES IN UNIT 2"",
                ""PAO_START_NUMBER"": ""1"",
                ""PAO_END_NUMBER"": ""10"",
                ""PAO_TEXT"": ""EXAMPLE STUDIOS"",
                ""STREET_DESCRIPTION"": ""EXAMPLE ROAD"",
                ""TOWN_NAME"": ""LONDON"",
                ""ADMINISTRATIVE_AREA"": ""CAMDEN"",
                ""POSTCODE_LOCATOR"": ""AB1 2CD"",
                ""RPC"": ""2"",
                ""X_COORDINATE"": 1234567.78,
                ""Y_COORDINATE"": 1234567.78,
                ""STATUS"": ""APPROVED"",
                ""LOGICAL_STATUS_CODE"": ""1"",
                ""CLASSIFICATION_CODE"": ""RD02"",
                ""CLASSIFICATION_CODE_DESCRIPTION"": ""Children’s Nursery / Crèche"",
                ""LOCAL_CUSTODIAN_CODE"": 5210,
                ""LOCAL_CUSTODIAN_CODE_DESCRIPTION"": ""CAMDEN"",
                ""COUNTRY_CODE"": ""E"",
                ""COUNTRY_CODE_DESCRIPTION"": ""This record is within England"",
                ""POSTAL_ADDRESS_CODE"": ""C"",
                ""POSTAL_ADDRESS_CODE_DESCRIPTION"": ""A record which is postal and has a parent record which is linked to PAF"",
                ""BLPU_STATE_CODE"": ""2"",
                ""BLPU_STATE_CODE_DESCRIPTION"": ""In use"",
                ""TOPOGRAPHY_LAYER_TOID"": ""osgb1000000000000"",
                ""PARENT_UPRN"": ""9090909"",
                ""LAST_UPDATE_DATE"": ""25/03/2022"",
                ""ENTRY_DATE"": ""08/10/2002"",
                ""BLPU_STATE_DATE"": ""08/10/2002"",
                ""STREET_STATE_CODE"": ""2"",
                ""STREET_STATE_CODE_DESCRIPTION"": ""Open"",
                ""STREET_CLASSIFICATION_CODE"": ""8"",
                ""STREET_CLASSIFICATION_CODE_DESCRIPTION"": ""All vehicles"",
                ""LPI_LOGICAL_STATUS_CODE"": ""1"",
                ""LPI_LOGICAL_STATUS_CODE_DESCRIPTION"": ""APPROVED"",
                ""LANGUAGE"": ""EN"",
                ""MATCH"": 1.0,
                ""MATCH_DESCRIPTION"": ""EXACT""
            }
        },
        {
            ""LPI"": {
                ""UPRN"": ""12345678905"",
                ""ADDRESS"": ""PREMISES IN UNIT 3, EXAMPLE STUDIOS, 1-10, EXAMPLE ROAD, LONDON, CAMDEN, AB1 2CD"",
                ""USRN"": ""9876543"",
                ""LPI_KEY"": ""5150L000099999"",
                ""SAO_TEXT"": ""PREMISES IN UNIT 3"",
                ""PAO_START_NUMBER"": ""1"",
                ""PAO_END_NUMBER"": ""10"",
                ""PAO_TEXT"": ""EXAMPLE STUDIOS"",
                ""STREET_DESCRIPTION"": ""EXAMPLE ROAD"",
                ""TOWN_NAME"": ""LONDON"",
                ""ADMINISTRATIVE_AREA"": ""CAMDEN"",
                ""POSTCODE_LOCATOR"": ""AB1 2CD"",
                ""RPC"": ""2"",
                ""X_COORDINATE"": 1234567.78,
                ""Y_COORDINATE"": 1234567.78,
                ""STATUS"": ""APPROVED"",
                ""LOGICAL_STATUS_CODE"": ""1"",
                ""CLASSIFICATION_CODE"": ""RD01"",
                ""CLASSIFICATION_CODE_DESCRIPTION"": ""Office / Work Studio"",
                ""LOCAL_CUSTODIAN_CODE"": 5210,
                ""LOCAL_CUSTODIAN_CODE_DESCRIPTION"": ""CAMDEN"",
                ""COUNTRY_CODE"": ""E"",
                ""COUNTRY_CODE_DESCRIPTION"": ""This record is within England"",
                ""POSTAL_ADDRESS_CODE"": ""C"",
                ""POSTAL_ADDRESS_CODE_DESCRIPTION"": ""A record which is postal and has a parent record which is linked to PAF"",
                ""BLPU_STATE_CODE"": ""2"",
                ""BLPU_STATE_CODE_DESCRIPTION"": ""In use"",
                ""TOPOGRAPHY_LAYER_TOID"": ""osgb1000000000000"",
                ""PARENT_UPRN"": ""9090909"",
                ""LAST_UPDATE_DATE"": ""25/03/2022"",
                ""ENTRY_DATE"": ""16/10/2012"",
                ""BLPU_STATE_DATE"": ""16/10/2012"",
                ""STREET_STATE_CODE"": ""2"",
                ""STREET_STATE_CODE_DESCRIPTION"": ""Open"",
                ""STREET_CLASSIFICATION_CODE"": ""8"",
                ""STREET_CLASSIFICATION_CODE_DESCRIPTION"": ""All vehicles"",
                ""LPI_LOGICAL_STATUS_CODE"": ""1"",
                ""LPI_LOGICAL_STATUS_CODE_DESCRIPTION"": ""APPROVED"",
                ""LANGUAGE"": ""EN"",
                ""MATCH"": 1.0,
                ""MATCH_DESCRIPTION"": ""EXACT""
            }
        }
    ]
}";
    
    private const string DummyOsPlacesResponseWithDpaAndLpiResults = @"{
    ""header"": {
        ""uri"": ""https://api.os.uk/search/places/v1/postcode?postcode=AB1%202CD&lr=EN&dataset=LPI%2CDPA"",
        ""query"": ""postcode=AB1 2CD"",
        ""offset"": 0,
        ""totalresults"": 6,
        ""format"": ""JSON"",
        ""dataset"": ""LPI,DPA"",
        ""lr"": ""EN"",
        ""maxresults"": 100,
        ""epoch"": ""102"",
        ""lastupdate"": ""2023-06-22"",
        ""output_srs"": ""EPSG:27700""
    },
    ""results"": [
        {
            ""DPA"": {
                ""UPRN"": ""12345671"",
                ""UDPRN"": ""12345671"",
                ""ADDRESS"": ""EXAMPLE HOUSE, EXAMPLE ROAD, LONDON, AB1 2CD"",
                ""BUILDING_NAME"": ""EXAMPLE HOUSE"",
                ""THOROUGHFARE_NAME"": ""EXAMPLE ROAD"",
                ""POST_TOWN"": ""LONDON"",
                ""POSTCODE"": ""AB1 2CD"",
                ""RPC"": ""2"",
                ""X_COORDINATE"": 1234567.78,
                ""Y_COORDINATE"": 1234567.78,
                ""STATUS"": ""APPROVED"",
                ""LOGICAL_STATUS_CODE"": ""1"",
                ""CLASSIFICATION_CODE"": ""RD01"",
                ""CLASSIFICATION_CODE_DESCRIPTION"": ""Property Shell"",
                ""LOCAL_CUSTODIAN_CODE"": 5150,
                ""LOCAL_CUSTODIAN_CODE_DESCRIPTION"": ""BRENT"",
                ""COUNTRY_CODE"": ""E"",
                ""COUNTRY_CODE_DESCRIPTION"": ""This record is within England"",
                ""POSTAL_ADDRESS_CODE"": ""D"",
                ""POSTAL_ADDRESS_CODE_DESCRIPTION"": ""A record which is linked to PAF"",
                ""BLPU_STATE_CODE"": ""2"",
                ""BLPU_STATE_CODE_DESCRIPTION"": ""In use"",
                ""TOPOGRAPHY_LAYER_TOID"": ""osgb1000009999999"",
                ""LAST_UPDATE_DATE"": ""30/07/2017"",
                ""ENTRY_DATE"": ""18/09/2007"",
                ""BLPU_STATE_DATE"": ""01/04/2009"",
                ""LANGUAGE"": ""EN"",
                ""MATCH"": 1.0,
                ""MATCH_DESCRIPTION"": ""EXACT"",
                ""DELIVERY_POINT_SUFFIX"": ""1A""
            }
        },
        {
            ""LPI"": {
                ""UPRN"": ""12345673"",
                ""ADDRESS"": ""4, EXAMPLE HOUSE, EXAMPLE ROAD, LONDON, BRENT, AB1 2CD"",
                ""USRN"": ""9876543"",
                ""LPI_KEY"": ""5150L000099999"",
                ""SAO_START_NUMBER"": ""4"",
                ""PAO_TEXT"": ""EXAMPLE HOUSE"",
                ""STREET_DESCRIPTION"": ""EXAMPLE ROAD"",
                ""TOWN_NAME"": ""LONDON"",
                ""ADMINISTRATIVE_AREA"": ""BRENT"",
                ""POSTCODE_LOCATOR"": ""AB1 2CD"",
                ""RPC"": ""2"",
                ""X_COORDINATE"": 1234567.78,
                ""Y_COORDINATE"": 1234567.78,
                ""STATUS"": ""APPROVED"",
                ""LOGICAL_STATUS_CODE"": ""1"",
                ""CLASSIFICATION_CODE"": ""RD06"",
                ""CLASSIFICATION_CODE_DESCRIPTION"": ""Self Contained Flat (Includes Maisonette / Apartment)"",
                ""LOCAL_CUSTODIAN_CODE"": 5150,
                ""LOCAL_CUSTODIAN_CODE_DESCRIPTION"": ""BRENT"",
                ""COUNTRY_CODE"": ""E"",
                ""COUNTRY_CODE_DESCRIPTION"": ""This record is within England"",
                ""POSTAL_ADDRESS_CODE"": ""C"",
                ""POSTAL_ADDRESS_CODE_DESCRIPTION"": ""A record which is postal and has a parent record which is linked to PAF"",
                ""BLPU_STATE_CODE"": ""2"",
                ""BLPU_STATE_CODE_DESCRIPTION"": ""In use"",
                ""TOPOGRAPHY_LAYER_TOID"": ""osgb1000009999999"",
                ""PARENT_UPRN"": ""12345671"",
                ""LAST_UPDATE_DATE"": ""30/07/2017"",
                ""ENTRY_DATE"": ""17/09/1979"",
                ""BLPU_STATE_DATE"": ""10/08/2007"",
                ""STREET_STATE_CODE"": ""2"",
                ""STREET_STATE_CODE_DESCRIPTION"": ""Open"",
                ""STREET_CLASSIFICATION_CODE"": ""8"",
                ""STREET_CLASSIFICATION_CODE_DESCRIPTION"": ""All vehicles"",
                ""LPI_LOGICAL_STATUS_CODE"": ""1"",
                ""LPI_LOGICAL_STATUS_CODE_DESCRIPTION"": ""APPROVED"",
                ""LANGUAGE"": ""EN"",
                ""MATCH"": 1.0,
                ""MATCH_DESCRIPTION"": ""EXACT""
            }
        },
        {
            ""LPI"": {
                ""UPRN"": ""12345674"",
                ""ADDRESS"": ""3, EXAMPLE HOUSE, EXAMPLE ROAD, LONDON, BRENT, AB1 2CD"",
                ""USRN"": ""9876543"",
                ""LPI_KEY"": ""5150L000099999"",
                ""SAO_START_NUMBER"": ""3"",
                ""PAO_TEXT"": ""EXAMPLE HOUSE"",
                ""STREET_DESCRIPTION"": ""EXAMPLE ROAD"",
                ""TOWN_NAME"": ""LONDON"",
                ""ADMINISTRATIVE_AREA"": ""BRENT"",
                ""POSTCODE_LOCATOR"": ""AB1 2CD"",
                ""RPC"": ""2"",
                ""X_COORDINATE"": 1234567.78,
                ""Y_COORDINATE"": 1234567.78,
                ""STATUS"": ""APPROVED"",
                ""LOGICAL_STATUS_CODE"": ""1"",
                ""CLASSIFICATION_CODE"": ""RD06"",
                ""CLASSIFICATION_CODE_DESCRIPTION"": ""Self Contained Flat (Includes Maisonette / Apartment)"",
                ""LOCAL_CUSTODIAN_CODE"": 5150,
                ""LOCAL_CUSTODIAN_CODE_DESCRIPTION"": ""BRENT"",
                ""COUNTRY_CODE"": ""E"",
                ""COUNTRY_CODE_DESCRIPTION"": ""This record is within England"",
                ""POSTAL_ADDRESS_CODE"": ""C"",
                ""POSTAL_ADDRESS_CODE_DESCRIPTION"": ""A record which is postal and has a parent record which is linked to PAF"",
                ""BLPU_STATE_CODE"": ""2"",
                ""BLPU_STATE_CODE_DESCRIPTION"": ""In use"",
                ""TOPOGRAPHY_LAYER_TOID"": ""osgb1000009999999"",
                ""PARENT_UPRN"": ""12345671"",
                ""LAST_UPDATE_DATE"": ""30/07/2017"",
                ""ENTRY_DATE"": ""17/09/1979"",
                ""BLPU_STATE_DATE"": ""10/08/2007"",
                ""STREET_STATE_CODE"": ""2"",
                ""STREET_STATE_CODE_DESCRIPTION"": ""Open"",
                ""STREET_CLASSIFICATION_CODE"": ""8"",
                ""STREET_CLASSIFICATION_CODE_DESCRIPTION"": ""All vehicles"",
                ""LPI_LOGICAL_STATUS_CODE"": ""1"",
                ""LPI_LOGICAL_STATUS_CODE_DESCRIPTION"": ""APPROVED"",
                ""LANGUAGE"": ""EN"",
                ""MATCH"": 1.0,
                ""MATCH_DESCRIPTION"": ""EXACT""
            }
        },
        {
            ""LPI"": {
                ""UPRN"": ""12345675"",
                ""ADDRESS"": ""2, EXAMPLE HOUSE, EXAMPLE ROAD, LONDON, BRENT, AB1 2CD"",
                ""USRN"": ""9876543"",
                ""LPI_KEY"": ""5150L000099999"",
                ""SAO_START_NUMBER"": ""2"",
                ""PAO_TEXT"": ""EXAMPLE HOUSE"",
                ""STREET_DESCRIPTION"": ""EXAMPLE ROAD"",
                ""TOWN_NAME"": ""LONDON"",
                ""ADMINISTRATIVE_AREA"": ""BRENT"",
                ""POSTCODE_LOCATOR"": ""AB1 2CD"",
                ""RPC"": ""2"",
                ""X_COORDINATE"": 1234567.78,
                ""Y_COORDINATE"": 1234567.78,
                ""STATUS"": ""APPROVED"",
                ""LOGICAL_STATUS_CODE"": ""1"",
                ""CLASSIFICATION_CODE"": ""RD06"",
                ""CLASSIFICATION_CODE_DESCRIPTION"": ""Self Contained Flat (Includes Maisonette / Apartment)"",
                ""LOCAL_CUSTODIAN_CODE"": 5150,
                ""LOCAL_CUSTODIAN_CODE_DESCRIPTION"": ""BRENT"",
                ""COUNTRY_CODE"": ""E"",
                ""COUNTRY_CODE_DESCRIPTION"": ""This record is within England"",
                ""POSTAL_ADDRESS_CODE"": ""C"",
                ""POSTAL_ADDRESS_CODE_DESCRIPTION"": ""A record which is postal and has a parent record which is linked to PAF"",
                ""BLPU_STATE_CODE"": ""2"",
                ""BLPU_STATE_CODE_DESCRIPTION"": ""In use"",
                ""TOPOGRAPHY_LAYER_TOID"": ""osgb1000009999999"",
                ""PARENT_UPRN"": ""12345671"",
                ""LAST_UPDATE_DATE"": ""30/07/2017"",
                ""ENTRY_DATE"": ""17/09/1979"",
                ""BLPU_STATE_DATE"": ""10/08/2007"",
                ""STREET_STATE_CODE"": ""2"",
                ""STREET_STATE_CODE_DESCRIPTION"": ""Open"",
                ""STREET_CLASSIFICATION_CODE"": ""8"",
                ""STREET_CLASSIFICATION_CODE_DESCRIPTION"": ""All vehicles"",
                ""LPI_LOGICAL_STATUS_CODE"": ""1"",
                ""LPI_LOGICAL_STATUS_CODE_DESCRIPTION"": ""APPROVED"",
                ""LANGUAGE"": ""EN"",
                ""MATCH"": 1.0,
                ""MATCH_DESCRIPTION"": ""EXACT""
            }
        },
        {
            ""LPI"": {
                ""UPRN"": ""12345676"",
                ""ADDRESS"": ""1, EXAMPLE HOUSE, EXAMPLE ROAD, LONDON, BRENT, AB1 2CD"",
                ""USRN"": ""9876543"",
                ""LPI_KEY"": ""5150L000099999"",
                ""SAO_START_NUMBER"": ""1"",
                ""PAO_TEXT"": ""EXAMPLE HOUSE"",
                ""STREET_DESCRIPTION"": ""EXAMPLE ROAD"",
                ""TOWN_NAME"": ""LONDON"",
                ""ADMINISTRATIVE_AREA"": ""BRENT"",
                ""POSTCODE_LOCATOR"": ""AB1 2CD"",
                ""RPC"": ""2"",
                ""X_COORDINATE"": 1234567.78,
                ""Y_COORDINATE"": 1234567.78,
                ""STATUS"": ""APPROVED"",
                ""LOGICAL_STATUS_CODE"": ""1"",
                ""CLASSIFICATION_CODE"": ""RD06"",
                ""CLASSIFICATION_CODE_DESCRIPTION"": ""Self Contained Flat (Includes Maisonette / Apartment)"",
                ""LOCAL_CUSTODIAN_CODE"": 5150,
                ""LOCAL_CUSTODIAN_CODE_DESCRIPTION"": ""BRENT"",
                ""COUNTRY_CODE"": ""E"",
                ""COUNTRY_CODE_DESCRIPTION"": ""This record is within England"",
                ""POSTAL_ADDRESS_CODE"": ""C"",
                ""POSTAL_ADDRESS_CODE_DESCRIPTION"": ""A record which is postal and has a parent record which is linked to PAF"",
                ""BLPU_STATE_CODE"": ""2"",
                ""BLPU_STATE_CODE_DESCRIPTION"": ""In use"",
                ""TOPOGRAPHY_LAYER_TOID"": ""osgb1000009999999"",
                ""PARENT_UPRN"": ""12345671"",
                ""LAST_UPDATE_DATE"": ""30/07/2017"",
                ""ENTRY_DATE"": ""17/09/1979"",
                ""BLPU_STATE_DATE"": ""10/08/2007"",
                ""STREET_STATE_CODE"": ""2"",
                ""STREET_STATE_CODE_DESCRIPTION"": ""Open"",
                ""STREET_CLASSIFICATION_CODE"": ""8"",
                ""STREET_CLASSIFICATION_CODE_DESCRIPTION"": ""All vehicles"",
                ""LPI_LOGICAL_STATUS_CODE"": ""1"",
                ""LPI_LOGICAL_STATUS_CODE_DESCRIPTION"": ""APPROVED"",
                ""LANGUAGE"": ""EN"",
                ""MATCH"": 1.0,
                ""MATCH_DESCRIPTION"": ""EXACT""
            }
        },
        {
            ""LPI"": {
                ""UPRN"": ""12345671"",
                ""ADDRESS"": ""EXAMPLE HOUSE, EXAMPLE ROAD, LONDON, BRENT, AB1 2CD"",
                ""USRN"": ""9876543"",
                ""LPI_KEY"": ""5150L000099999"",
                ""PAO_TEXT"": ""EXAMPLE HOUSE"",
                ""STREET_DESCRIPTION"": ""EXAMPLE ROAD"",
                ""TOWN_NAME"": ""LONDON"",
                ""ADMINISTRATIVE_AREA"": ""BRENT"",
                ""POSTCODE_LOCATOR"": ""AB1 2CD"",
                ""RPC"": ""2"",
                ""X_COORDINATE"": 1234567.78,
                ""Y_COORDINATE"": 1234567.78,
                ""STATUS"": ""APPROVED"",
                ""LOGICAL_STATUS_CODE"": ""1"",
                ""CLASSIFICATION_CODE"": ""PP"",
                ""CLASSIFICATION_CODE_DESCRIPTION"": ""Property Shell"",
                ""LOCAL_CUSTODIAN_CODE"": 5150,
                ""LOCAL_CUSTODIAN_CODE_DESCRIPTION"": ""BRENT"",
                ""COUNTRY_CODE"": ""E"",
                ""COUNTRY_CODE_DESCRIPTION"": ""This record is within England"",
                ""POSTAL_ADDRESS_CODE"": ""D"",
                ""POSTAL_ADDRESS_CODE_DESCRIPTION"": ""A record which is linked to PAF"",
                ""BLPU_STATE_CODE"": ""2"",
                ""BLPU_STATE_CODE_DESCRIPTION"": ""In use"",
                ""TOPOGRAPHY_LAYER_TOID"": ""osgb1000009999999"",
                ""LAST_UPDATE_DATE"": ""30/07/2017"",
                ""ENTRY_DATE"": ""18/09/2007"",
                ""BLPU_STATE_DATE"": ""01/04/2009"",
                ""STREET_STATE_CODE"": ""2"",
                ""STREET_STATE_CODE_DESCRIPTION"": ""Open"",
                ""STREET_CLASSIFICATION_CODE"": ""8"",
                ""STREET_CLASSIFICATION_CODE_DESCRIPTION"": ""All vehicles"",
                ""LPI_LOGICAL_STATUS_CODE"": ""1"",
                ""LPI_LOGICAL_STATUS_CODE_DESCRIPTION"": ""APPROVED"",
                ""LANGUAGE"": ""EN"",
                ""MATCH"": 1.0,
                ""MATCH_DESCRIPTION"": ""EXACT""
            }
        }
    ]
}";
    
    // First property postal address code = N
    // Second property logical status code = 0
    // Third property classification code = CO
    // Fourth is OK
    private const string DummyOsPlacesResponseWithNonResidentialResults = @"{
    ""header"": {
        ""uri"" : ""https://api.os.uk/search/places/v1/postcode?postcode=AB1%202CD"",
        ""query"" : ""postcode=AB1 2CD"",
        ""offset"" : 0,
        ""totalresults"" : 5,
        ""format"" : ""JSON"",
        ""dataset"" : ""LPI"",
        ""lr"" : ""EN,CY"",
        ""maxresults"" : 100,
        ""epoch"" : ""101"",
        ""lastupdate"" : ""2023-05-18"",
        ""output_srs"" : ""EPSG:27700""
      },
    ""results"": [
        {
            ""LPI"": {
                ""UPRN"": ""12345678901"",
                ""ADDRESS"": ""CAFE, EXAMPLE STUDIOS, 1-10, EXAMPLE ROAD, LONDON, CAMDEN, AB1 2CD"",
                ""USRN"": ""9876543"",
                ""LPI_KEY"": ""5150L000099999"",
                ""SAO_TEXT"": ""CAFE"",
                ""PAO_START_NUMBER"": ""1"",
                ""PAO_END_NUMBER"": ""10"",
                ""PAO_TEXT"": ""EXAMPLE STUDIOS"",
                ""STREET_DESCRIPTION"": ""EXAMPLE ROAD"",
                ""TOWN_NAME"": ""LONDON"",
                ""ADMINISTRATIVE_AREA"": ""CAMDEN"",
                ""POSTCODE_LOCATOR"": ""AB1 2CD"",
                ""RPC"": ""2"",
                ""X_COORDINATE"": 1234567.78,
                ""Y_COORDINATE"": 1234567.78,
                ""STATUS"": ""APPROVED"",
                ""LOGICAL_STATUS_CODE"": ""1"",
                ""CLASSIFICATION_CODE"": ""RD07"",
                ""CLASSIFICATION_CODE_DESCRIPTION"": ""Restaurant / Cafeteria"",
                ""LOCAL_CUSTODIAN_CODE"": 5210,
                ""LOCAL_CUSTODIAN_CODE_DESCRIPTION"": ""CAMDEN"",
                ""COUNTRY_CODE"": ""E"",
                ""COUNTRY_CODE_DESCRIPTION"": ""This record is within England"",
                ""POSTAL_ADDRESS_CODE"": ""N"",
                ""POSTAL_ADDRESS_CODE_DESCRIPTION"": ""A record which is postal and has a parent record which is linked to PAF"",
                ""BLPU_STATE_CODE"": ""2"",
                ""BLPU_STATE_CODE_DESCRIPTION"": ""In use"",
                ""TOPOGRAPHY_LAYER_TOID"": ""osgb1000000000000"",
                ""PARENT_UPRN"": ""9090909"",
                ""LAST_UPDATE_DATE"": ""25/03/2022"",
                ""ENTRY_DATE"": ""08/10/2002"",
                ""BLPU_STATE_DATE"": ""08/10/2002"",
                ""STREET_STATE_CODE"": ""2"",
                ""STREET_STATE_CODE_DESCRIPTION"": ""Open"",
                ""STREET_CLASSIFICATION_CODE"": ""8"",
                ""STREET_CLASSIFICATION_CODE_DESCRIPTION"": ""All vehicles"",
                ""LPI_LOGICAL_STATUS_CODE"": ""1"",
                ""LPI_LOGICAL_STATUS_CODE_DESCRIPTION"": ""APPROVED"",
                ""LANGUAGE"": ""EN"",
                ""MATCH"": 1.0,
                ""MATCH_DESCRIPTION"": ""EXACT""
            }
        },
        {
            ""LPI"": {
                ""UPRN"": ""12345678902"",
                ""ADDRESS"": ""CAR PARKING SPACES, EXAMPLE STUDIOS, 1-10, EXAMPLE ROAD, LONDON, CAMDEN, AB1 2CD"",
                ""USRN"": ""9876543"",
                ""LPI_KEY"": ""5150L000099999"",
                ""SAO_TEXT"": ""CAR PARKING SPACES"",
                ""PAO_START_NUMBER"": ""1"",
                ""PAO_END_NUMBER"": ""10"",
                ""PAO_TEXT"": ""EXAMPLE STUDIOS"",
                ""STREET_DESCRIPTION"": ""EXAMPLE ROAD"",
                ""TOWN_NAME"": ""LONDON"",
                ""ADMINISTRATIVE_AREA"": ""CAMDEN"",
                ""POSTCODE_LOCATOR"": ""AB1 2CD"",
                ""RPC"": ""2"",
                ""X_COORDINATE"": 1234567.78,
                ""Y_COORDINATE"": 1234567.78,
                ""STATUS"": ""APPROVED"",
                ""LOGICAL_STATUS_CODE"": ""1"",
                ""CLASSIFICATION_CODE"": ""RD07"",
                ""CLASSIFICATION_CODE_DESCRIPTION"": ""Commercial"",
                ""LOCAL_CUSTODIAN_CODE"": 5210,
                ""LOCAL_CUSTODIAN_CODE_DESCRIPTION"": ""CAMDEN"",
                ""COUNTRY_CODE"": ""E"",
                ""COUNTRY_CODE_DESCRIPTION"": ""This record is within England"",
                ""POSTAL_ADDRESS_CODE"": ""C"",
                ""POSTAL_ADDRESS_CODE_DESCRIPTION"": ""A record which is postal and has a parent record which is linked to PAF"",
                ""BLPU_STATE_CODE"": ""2"",
                ""BLPU_STATE_CODE_DESCRIPTION"": ""In use"",
                ""TOPOGRAPHY_LAYER_TOID"": ""osgb1000000000000"",
                ""PARENT_UPRN"": ""9090909"",
                ""LAST_UPDATE_DATE"": ""25/03/2022"",
                ""ENTRY_DATE"": ""08/10/2002"",
                ""BLPU_STATE_DATE"": ""08/10/2002"",
                ""STREET_STATE_CODE"": ""2"",
                ""STREET_STATE_CODE_DESCRIPTION"": ""Open"",
                ""STREET_CLASSIFICATION_CODE"": ""8"",
                ""STREET_CLASSIFICATION_CODE_DESCRIPTION"": ""All vehicles"",
                ""LPI_LOGICAL_STATUS_CODE"": ""0"",
                ""LPI_LOGICAL_STATUS_CODE_DESCRIPTION"": ""APPROVED"",
                ""LANGUAGE"": ""EN"",
                ""MATCH"": 1.0,
                ""MATCH_DESCRIPTION"": ""EXACT""
            }
        },
        {
            ""LPI"": {
                ""UPRN"": ""12345678903"",
                ""ADDRESS"": ""EXAMPLE ORGANISATION, PREMISES IN UNIT 1, EXAMPLE STUDIOS, 1-10, EXAMPLE ROAD, LONDON, CAMDEN, AB1 2CD"",
                ""USRN"": ""9876543"",
                ""LPI_KEY"": ""5150L000099999"",
                ""ORGANISATION"": ""EXAMPLE ORGANISATION"",
                ""SAO_TEXT"": ""PREMISES IN UNIT 1"",
                ""PAO_START_NUMBER"": ""1"",
                ""PAO_END_NUMBER"": ""10"",
                ""PAO_TEXT"": ""EXAMPLE STUDIOS"",
                ""STREET_DESCRIPTION"": ""EXAMPLE ROAD"",
                ""TOWN_NAME"": ""LONDON"",
                ""ADMINISTRATIVE_AREA"": ""CAMDEN"",
                ""POSTCODE_LOCATOR"": ""AB1 2CD"",
                ""RPC"": ""2"",
                ""X_COORDINATE"": 1234567.78,
                ""Y_COORDINATE"": 1234567.78,
                ""STATUS"": ""APPROVED"",
                ""LOGICAL_STATUS_CODE"": ""1"",
                ""CLASSIFICATION_CODE"": ""CO06"",
                ""CLASSIFICATION_CODE_DESCRIPTION"": ""Indoor / Outdoor Leisure / Sporting Activity / Centre"",
                ""LOCAL_CUSTODIAN_CODE"": 5210,
                ""LOCAL_CUSTODIAN_CODE_DESCRIPTION"": ""CAMDEN"",
                ""COUNTRY_CODE"": ""E"",
                ""COUNTRY_CODE_DESCRIPTION"": ""This record is within England"",
                ""POSTAL_ADDRESS_CODE"": ""D"",
                ""POSTAL_ADDRESS_CODE_DESCRIPTION"": ""A record which is linked to PAF"",
                ""BLPU_STATE_CODE"": ""2"",
                ""BLPU_STATE_CODE_DESCRIPTION"": ""In use"",
                ""TOPOGRAPHY_LAYER_TOID"": ""osgb1000000000000"",
                ""PARENT_UPRN"": ""9090909"",
                ""LAST_UPDATE_DATE"": ""25/03/2022"",
                ""ENTRY_DATE"": ""08/10/2002"",
                ""BLPU_STATE_DATE"": ""08/10/2002"",
                ""STREET_STATE_CODE"": ""2"",
                ""STREET_STATE_CODE_DESCRIPTION"": ""Open"",
                ""STREET_CLASSIFICATION_CODE"": ""8"",
                ""STREET_CLASSIFICATION_CODE_DESCRIPTION"": ""All vehicles"",
                ""LPI_LOGICAL_STATUS_CODE"": ""1"",
                ""LPI_LOGICAL_STATUS_CODE_DESCRIPTION"": ""APPROVED"",
                ""LANGUAGE"": ""EN"",
                ""MATCH"": 1.0,
                ""MATCH_DESCRIPTION"": ""EXACT""
            }
        },
        {
            ""LPI"": {
                ""UPRN"": ""12345678904"",
                ""ADDRESS"": ""PREMISES IN UNIT 2, EXAMPLE STUDIOS, 1-10, EXAMPLE ROAD, LONDON, CAMDEN, AB1 2CD"",
                ""USRN"": ""9876543"",
                ""LPI_KEY"": ""5150L000099999"",
                ""SAO_TEXT"": ""PREMISES IN UNIT 2"",
                ""PAO_START_NUMBER"": ""1"",
                ""PAO_END_NUMBER"": ""10"",
                ""PAO_TEXT"": ""EXAMPLE STUDIOS"",
                ""STREET_DESCRIPTION"": ""EXAMPLE ROAD"",
                ""TOWN_NAME"": ""LONDON"",
                ""ADMINISTRATIVE_AREA"": ""CAMDEN"",
                ""POSTCODE_LOCATOR"": ""AB1 2CD"",
                ""RPC"": ""2"",
                ""X_COORDINATE"": 1234567.78,
                ""Y_COORDINATE"": 1234567.78,
                ""STATUS"": ""APPROVED"",
                ""LOGICAL_STATUS_CODE"": ""1"",
                ""CLASSIFICATION_CODE"": ""RD02"",
                ""CLASSIFICATION_CODE_DESCRIPTION"": ""Children’s Nursery / Crèche"",
                ""LOCAL_CUSTODIAN_CODE"": 5210,
                ""LOCAL_CUSTODIAN_CODE_DESCRIPTION"": ""CAMDEN"",
                ""COUNTRY_CODE"": ""E"",
                ""COUNTRY_CODE_DESCRIPTION"": ""This record is within England"",
                ""POSTAL_ADDRESS_CODE"": ""C"",
                ""POSTAL_ADDRESS_CODE_DESCRIPTION"": ""A record which is postal and has a parent record which is linked to PAF"",
                ""BLPU_STATE_CODE"": ""2"",
                ""BLPU_STATE_CODE_DESCRIPTION"": ""In use"",
                ""TOPOGRAPHY_LAYER_TOID"": ""osgb1000000000000"",
                ""PARENT_UPRN"": ""9090909"",
                ""LAST_UPDATE_DATE"": ""25/03/2022"",
                ""ENTRY_DATE"": ""08/10/2002"",
                ""BLPU_STATE_DATE"": ""08/10/2002"",
                ""STREET_STATE_CODE"": ""2"",
                ""STREET_STATE_CODE_DESCRIPTION"": ""Open"",
                ""STREET_CLASSIFICATION_CODE"": ""8"",
                ""STREET_CLASSIFICATION_CODE_DESCRIPTION"": ""All vehicles"",
                ""LPI_LOGICAL_STATUS_CODE"": ""1"",
                ""LPI_LOGICAL_STATUS_CODE_DESCRIPTION"": ""APPROVED"",
                ""LANGUAGE"": ""EN"",
                ""MATCH"": 1.0,
                ""MATCH_DESCRIPTION"": ""EXACT""
            }
        }
    ]
}";
}
