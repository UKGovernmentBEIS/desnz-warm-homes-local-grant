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
    public async Task GetAddressesAsync_WithJustLpiResults_ParsesAddress()
    {
        // Arrange
        mockHttpHandler.Expect("http://test.com/search/places/v1/postcode?postcode=AB1%202CD")
            .WithHeaders("Key", "testKey")
            .Respond("application/json", DummyResponseWithJustLpiData);

        var correctAddresses = new List<Address>
        {
            new()
            {
                AddressLine1 = "85, Test Road",
                AddressLine2 = "",
                Town = "London",
                Postcode = "AB1 2CD",
                County = null,
                Uprn = "12345678904",
                LocalCustodianCode = "5600"
            },
            new()
            {
                AddressLine1 = "86, Test Road",
                AddressLine2 = "",
                Town = "London",
                Postcode = "AB1 2CD",
                County = null,
                Uprn = "12345678905",
                LocalCustodianCode = "5600"
            },
        };

        // Act
        var result = await underTest.GetAddressesAsync("AB1 2CD", "Non matching name");
        
        // Assert
        result.Should().BeEquivalentTo(correctAddresses);
    }
    
    [Test]
    public async Task GetAddressesAsync_WithLpiAndDpaResults_PrefersDpaResults()
    {
        // Arrange
        mockHttpHandler.Expect("http://test.com/search/places/v1/postcode?postcode=AB1%202CD")
            .WithHeaders("Key", "testKey")
            .Respond("application/json", DummyResponseWithMatchingLpiAndDpaData);

        var correctAddresses = new List<Address>
        {
            new()
            {
                AddressLine1 = "85, Test Road Dpa",
                AddressLine2 = "",
                Town = "London",
                Postcode = "AB1 2CD",
                County = null,
                Uprn = "12345678904",
                LocalCustodianCode = "5600"
            },
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
            .Respond("application/json", DummyResponseWithJustLpiData);

        // Act
        var result = await underTest.GetAddressesAsync("AB1 2CD", "85");
        
        // Assert
        result.Count.Should().Be(1);
        result.Single().AddressLine1.Should().Be("85, Test Road");
    }
    
    [Test]
    public async Task GetAddressesAsync_CalledWithoutMatchingHouseNumber_ReturnsAllResults()
    {
        // Arrange
        mockHttpHandler.Expect("http://test.com/search/places/v1/postcode?postcode=AB1%202CD")
            .WithHeaders("Key", "testKey")
            .Respond("application/json", DummyResponseWithJustLpiData);

        // Act
        var result = await underTest.GetAddressesAsync("AB1 2CD", "55");
        
        // Assert
        result.Count.Should().Be(2);
        result.Should().ContainSingle(a => a.AddressLine1 == "85, Test Road");
        result.Should().ContainSingle(a => a.AddressLine1 == "86, Test Road");
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
    private const string DummyResponseWithJustLpiData = @"{
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
    ""LPI"" : {
        ""UPRN"" : ""12345678904"",
        ""ADDRESS"" : ""85, TEST ROAD, LONDON, KENSINGTON AND CHELSEA, AB1 2CD"",
        ""USRN"" : ""21701204"",
        ""LPI_KEY"" : ""5600L000091363"",
        ""PAO_START_NUMBER"" : ""85"",
        ""STREET_DESCRIPTION"" : ""TEST ROAD"",
        ""TOWN_NAME"" : ""LONDON"",
        ""ADMINISTRATIVE_AREA"" : ""KENSINGTON AND CHELSEA"",
        ""POSTCODE_LOCATOR"" : ""AB1 2CD"",
        ""RPC"" : ""1"",
        ""X_COORDINATE"" : 645714.0,
        ""Y_COORDINATE"" : 789048.0,
        ""STATUS"" : ""APPROVED"",
        ""LOGICAL_STATUS_CODE"" : ""1"",
        ""CLASSIFICATION_CODE"" : ""RD04"",
        ""CLASSIFICATION_CODE_DESCRIPTION"" : ""Terraced"",
        ""LOCAL_CUSTODIAN_CODE"" : 5600,
        ""LOCAL_CUSTODIAN_CODE_DESCRIPTION"" : ""KENSINGTON AND CHELSEA"",
        ""COUNTRY_CODE"" : ""E"",
        ""COUNTRY_CODE_DESCRIPTION"" : ""This record is within England"",
        ""POSTAL_ADDRESS_CODE"" : ""C"",
        ""POSTAL_ADDRESS_CODE_DESCRIPTION"" : ""A record which is postal and has a parent record which is linked to PAF"",
        ""BLPU_STATE_CODE"" : ""2"",
        ""BLPU_STATE_CODE_DESCRIPTION"" : ""In use"",
        ""TOPOGRAPHY_LAYER_TOID"" : ""osgb1000049368881"",
        ""PARENT_UPRN"" : ""217059221"",
        ""LAST_UPDATE_DATE"" : ""22/06/2020"",
        ""ENTRY_DATE"" : ""14/12/1990"",
        ""BLPU_STATE_DATE"" : ""07/07/2015"",
        ""LPI_LOGICAL_STATUS_CODE"" : ""1"",
        ""LPI_LOGICAL_STATUS_CODE_DESCRIPTION"" : ""APPROVED"",
        ""LANGUAGE"" : ""EN"",
        ""MATCH"" : 1.0,
        ""MATCH_DESCRIPTION"" : ""EXACT""
    }
  },
  {
    ""LPI"" : {
        ""UPRN"" : ""12345678905"",
        ""ADDRESS"" : ""86, TEST ROAD, LONDON, KENSINGTON AND CHELSEA, AB1 2CD"",
        ""USRN"" : ""21701205"",
        ""LPI_KEY"" : ""5600L000091364"",
        ""PAO_START_NUMBER"" : ""86"",
        ""STREET_DESCRIPTION"" : ""TEST ROAD"",
        ""TOWN_NAME"" : ""LONDON"",
        ""ADMINISTRATIVE_AREA"" : ""KENSINGTON AND CHELSEA"",
        ""POSTCODE_LOCATOR"" : ""AB1 2CD"",
        ""RPC"" : ""1"",
        ""X_COORDINATE"" : 645715.0,
        ""Y_COORDINATE"" : 789047.0,
        ""STATUS"" : ""APPROVED"",
        ""LOGICAL_STATUS_CODE"" : ""1"",
        ""CLASSIFICATION_CODE"" : ""RD04"",
        ""CLASSIFICATION_CODE_DESCRIPTION"" : ""Terraced"",
        ""LOCAL_CUSTODIAN_CODE"" : 5600,
        ""LOCAL_CUSTODIAN_CODE_DESCRIPTION"" : ""KENSINGTON AND CHELSEA"",
        ""COUNTRY_CODE"" : ""E"",
        ""COUNTRY_CODE_DESCRIPTION"" : ""This record is within England"",
        ""POSTAL_ADDRESS_CODE"" : ""C"",
        ""POSTAL_ADDRESS_CODE_DESCRIPTION"" : ""A record which is postal and has a parent record which is linked to PAF"",
        ""BLPU_STATE_CODE"" : ""2"",
        ""BLPU_STATE_CODE_DESCRIPTION"" : ""In use"",
        ""TOPOGRAPHY_LAYER_TOID"" : ""osgb1000049368882"",
        ""PARENT_UPRN"" : ""217059222"",
        ""LAST_UPDATE_DATE"" : ""22/06/2020"",
        ""ENTRY_DATE"" : ""14/12/1990"",
        ""BLPU_STATE_DATE"" : ""07/07/2015"",
        ""LPI_LOGICAL_STATUS_CODE"" : ""1"",
        ""LPI_LOGICAL_STATUS_CODE_DESCRIPTION"" : ""APPROVED"",
        ""LANGUAGE"" : ""EN"",
        ""MATCH"" : 1.0,
        ""MATCH_DESCRIPTION"" : ""EXACT""
    }
  } ]
}";
    
     private const string DummyResponseWithMatchingLpiAndDpaData = @"{
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
    ""LPI"" : {
        ""UPRN"" : ""12345678904"",
        ""ADDRESS"" : ""85, TEST ROAD, LONDON, KENSINGTON AND CHELSEA, AB1 2CD"",
        ""USRN"" : ""21701204"",
        ""LPI_KEY"" : ""5600L000091363"",
        ""PAO_START_NUMBER"" : ""85"",
        ""STREET_DESCRIPTION"" : ""TEST ROAD"",
        ""TOWN_NAME"" : ""LONDON"",
        ""ADMINISTRATIVE_AREA"" : ""KENSINGTON AND CHELSEA"",
        ""POSTCODE_LOCATOR"" : ""AB1 2CD"",
        ""RPC"" : ""1"",
        ""X_COORDINATE"" : 645714.0,
        ""Y_COORDINATE"" : 789048.0,
        ""STATUS"" : ""APPROVED"",
        ""LOGICAL_STATUS_CODE"" : ""1"",
        ""CLASSIFICATION_CODE"" : ""RD04"",
        ""CLASSIFICATION_CODE_DESCRIPTION"" : ""Terraced"",
        ""LOCAL_CUSTODIAN_CODE"" : 5600,
        ""LOCAL_CUSTODIAN_CODE_DESCRIPTION"" : ""KENSINGTON AND CHELSEA"",
        ""COUNTRY_CODE"" : ""E"",
        ""COUNTRY_CODE_DESCRIPTION"" : ""This record is within England"",
        ""POSTAL_ADDRESS_CODE"" : ""C"",
        ""POSTAL_ADDRESS_CODE_DESCRIPTION"" : ""A record which is postal and has a parent record which is linked to PAF"",
        ""BLPU_STATE_CODE"" : ""2"",
        ""BLPU_STATE_CODE_DESCRIPTION"" : ""In use"",
        ""TOPOGRAPHY_LAYER_TOID"" : ""osgb1000049368881"",
        ""PARENT_UPRN"" : ""217059221"",
        ""LAST_UPDATE_DATE"" : ""22/06/2020"",
        ""ENTRY_DATE"" : ""14/12/1990"",
        ""BLPU_STATE_DATE"" : ""07/07/2015"",
        ""LPI_LOGICAL_STATUS_CODE"" : ""1"",
        ""LPI_LOGICAL_STATUS_CODE_DESCRIPTION"" : ""APPROVED"",
        ""LANGUAGE"" : ""EN"",
        ""MATCH"" : 1.0,
        ""MATCH_DESCRIPTION"" : ""EXACT""
    },
    ""DPA"" : {
      ""UPRN"" : ""12345678904"",
      ""UDPRN"" : ""12345671"",
      ""ADDRESS"" : ""85, TEST ROAD, LONDON, KENSINGTON AND CHELSEA, AB1 2CD"",
      ""BUILDING_NUMBER"" : ""85"",
      ""THOROUGHFARE_NAME"" : ""TEST ROAD DPA"",
      ""POST_TOWN"" : ""LONDON"",
      ""POSTCODE"" : ""AB1 2CD"",
      ""RPC"" : ""1"",
      ""X_COORDINATE"" : 123456.78,
      ""Y_COORDINATE"" : 123456.78,
      ""STATUS"" : ""APPROVED"",
      ""LOGICAL_STATUS_CODE"" : ""1"",
      ""CLASSIFICATION_CODE"" : ""RD04"",
      ""CLASSIFICATION_CODE_DESCRIPTION"" : ""Terraced"",
      ""LOCAL_CUSTODIAN_CODE"" : 5600,
      ""LOCAL_CUSTODIAN_CODE_DESCRIPTION"" : ""KENSINGTON AND CHELSEA"",
      ""COUNTRY_CODE"" : ""E"",
      ""COUNTRY_CODE_DESCRIPTION"" : ""This record is within England"",
      ""POSTAL_ADDRESS_CODE"" : ""C"",
      ""POSTAL_ADDRESS_CODE_DESCRIPTION"" : ""A record which is postal and has a parent record which is linked to PAF"",
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
    ""LPI"" : {
        ""UPRN"" : ""12345678904"",
        ""ADDRESS"" : ""85, TEST ROAD, LONDON, KENSINGTON AND CHELSEA, AB1 2CD"",
        ""USRN"" : ""21701204"",
        ""LPI_KEY"" : ""5600L000091363"",
        ""PAO_START_NUMBER"" : ""85"",
        ""STREET_DESCRIPTION"" : ""TEST ROAD"",
        ""TOWN_NAME"" : ""LONDON"",
        ""ADMINISTRATIVE_AREA"" : ""KENSINGTON AND CHELSEA"",
        ""POSTCODE_LOCATOR"" : ""AB1 2CD"",
        ""RPC"" : ""1"",
        ""X_COORDINATE"" : 645714.0,
        ""Y_COORDINATE"" : 789048.0,
        ""STATUS"" : ""APPROVED"",
        ""LOGICAL_STATUS_CODE"" : ""1"",
        ""CLASSIFICATION_CODE"" : ""RD04"",
        ""CLASSIFICATION_CODE_DESCRIPTION"" : ""Terraced"",
        ""LOCAL_CUSTODIAN_CODE"" : 5600,
        ""LOCAL_CUSTODIAN_CODE_DESCRIPTION"" : ""KENSINGTON AND CHELSEA"",
        ""COUNTRY_CODE"" : ""E"",
        ""COUNTRY_CODE_DESCRIPTION"" : ""This record is within England"",
        ""POSTAL_ADDRESS_CODE"" : ""C"",
        ""POSTAL_ADDRESS_CODE_DESCRIPTION"" : ""A record which is postal and has a parent record which is linked to PAF"",
        ""BLPU_STATE_CODE"" : ""2"",
        ""BLPU_STATE_CODE_DESCRIPTION"" : ""In use"",
        ""TOPOGRAPHY_LAYER_TOID"" : ""osgb1000049368881"",
        ""PARENT_UPRN"" : ""217059221"",
        ""LAST_UPDATE_DATE"" : ""22/06/2020"",
        ""ENTRY_DATE"" : ""14/12/1990"",
        ""BLPU_STATE_DATE"" : ""07/07/2015"",
        ""LPI_LOGICAL_STATUS_CODE"" : ""1"",
        ""LPI_LOGICAL_STATUS_CODE_DESCRIPTION"" : ""APPROVED"",
        ""LANGUAGE"" : ""EN"",
        ""MATCH"" : 1.0,
        ""MATCH_DESCRIPTION"" : ""EXACT""
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
    ""LPI"" : {
        ""UPRN"" : ""12345678905"",
        ""ADDRESS"" : ""86, TEST ROAD, LONDON, KENSINGTON AND CHELSEA, AB1 2CD"",
        ""USRN"" : ""21701205"",
        ""LPI_KEY"" : ""5600L000091364"",
        ""PAO_START_NUMBER"" : ""86"",
        ""STREET_DESCRIPTION"" : ""TEST ROAD"",
        ""TOWN_NAME"" : ""LONDON"",
        ""ADMINISTRATIVE_AREA"" : ""KENSINGTON AND CHELSEA"",
        ""POSTCODE_LOCATOR"" : ""AB1 2CD"",
        ""RPC"" : ""1"",
        ""X_COORDINATE"" : 645715.0,
        ""Y_COORDINATE"" : 789047.0,
        ""STATUS"" : ""APPROVED"",
        ""LOGICAL_STATUS_CODE"" : ""1"",
        ""CLASSIFICATION_CODE"" : ""RD04"",
        ""CLASSIFICATION_CODE_DESCRIPTION"" : ""Terraced"",
        ""LOCAL_CUSTODIAN_CODE"" : 5600,
        ""LOCAL_CUSTODIAN_CODE_DESCRIPTION"" : ""KENSINGTON AND CHELSEA"",
        ""COUNTRY_CODE"" : ""E"",
        ""COUNTRY_CODE_DESCRIPTION"" : ""This record is within England"",
        ""POSTAL_ADDRESS_CODE"" : ""C"",
        ""POSTAL_ADDRESS_CODE_DESCRIPTION"" : ""A record which is postal and has a parent record which is linked to PAF"",
        ""BLPU_STATE_CODE"" : ""2"",
        ""BLPU_STATE_CODE_DESCRIPTION"" : ""In use"",
        ""TOPOGRAPHY_LAYER_TOID"" : ""osgb1000049368882"",
        ""PARENT_UPRN"" : ""217059222"",
        ""LAST_UPDATE_DATE"" : ""22/06/2020"",
        ""ENTRY_DATE"" : ""14/12/1990"",
        ""BLPU_STATE_DATE"" : ""07/07/2015"",
        ""LPI_LOGICAL_STATUS_CODE"" : ""1"",
        ""LPI_LOGICAL_STATUS_CODE_DESCRIPTION"" : ""APPROVED"",
        ""LANGUAGE"" : ""EN"",
        ""MATCH"" : 1.0,
        ""MATCH_DESCRIPTION"" : ""EXACT""
    }
  } ]
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
