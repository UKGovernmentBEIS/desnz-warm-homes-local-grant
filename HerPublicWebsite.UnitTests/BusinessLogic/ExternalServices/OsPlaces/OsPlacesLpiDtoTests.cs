using FluentAssertions;
using HerPublicWebsite.BusinessLogic.ExternalServices.OsPlaces;
using NUnit.Framework;

namespace Tests.BusinessLogic.ExternalServices.OsPlaces;

[TestFixture]
public class OsPlacesLpiDtoTests
{
    [Test]
    public void Parse_WithOrganisation_PutsOrganisationFirst()
    {
        // Arrange
        var underTest = new OsPlacesLpiDto
        {
            Organisation = "EXAMPLE ORGANISATION",
            PaoText = "UNIT 1",
            StreetDescription = "EXAMPLE ROAD",
            TownName = "EXAMPLETON",
            AdministrativeArea = "EXAMPLESHIRE",
            PostcodeLocator = "AB1 2CD",
            Uprn = "123456789012",
            LocalCustodianCode = "123"
        };
        
        // Act
        var result = underTest.Parse();
        
        // Assert
        result.AddressLine1.Should().Be("Example Organisation, Unit 1");
        result.AddressLine2.Should().Be("Example Road");
        result.Town.Should().Be("Exampleton");
        result.Postcode.Should().Be("AB1 2CD");
        result.Uprn.Should().Be("123456789012");
        result.LocalCustodianCode.Should().Be("123");
    }
    
    [Test]
    public void Parse_WithSaoText_PutsSaoTextBeforePaoDetails()
    {
        // Arrange
        var underTest = new OsPlacesLpiDto
        {
            SaoText = "FLAT 1",
            PaoStartNumber = "1",
            PaoText = "EXAMPLE HOUSE",
            StreetDescription = "EXAMPLE ROAD",
            TownName = "EXAMPLETON",
            AdministrativeArea = "EXAMPLESHIRE",
            PostcodeLocator = "AB1 2CD",
            Uprn = "123456789012",
            LocalCustodianCode = "123"
        };
        
        // Act
        var result = underTest.Parse();
        
        // Assert
        result.AddressLine1.Should().Be("Flat 1, Example House");
        result.AddressLine2.Should().Be("1, Example Road");
        result.Town.Should().Be("Exampleton");
        result.Postcode.Should().Be("AB1 2CD");
        result.Uprn.Should().Be("123456789012");
        result.LocalCustodianCode.Should().Be("123");
    }
    
    [Test]
    public void Parse_WithPaoNumberRange_FormatsWithADash()
    {
        // Arrange
        var underTest = new OsPlacesLpiDto
        {
            SaoText = "EXAMPLE SAO",
            PaoStartNumber = "1",
            PaoEndNumber = "10",
            PaoText = "EXAMPLE HOUSE",
            StreetDescription = "EXAMPLE ROAD",
            TownName = "EXAMPLETON",
            AdministrativeArea = "EXAMPLESHIRE",
            PostcodeLocator = "AB1 2CD",
            Uprn = "123456789012",
            LocalCustodianCode = "123"
        };
        
        // Act
        var result = underTest.Parse();
        
        // Assert
        result.AddressLine1.Should().Be("Example Sao, Example House");
        result.AddressLine2.Should().Be("1–10, Example Road");
        result.Town.Should().Be("Exampleton");
        result.Postcode.Should().Be("AB1 2CD");
        result.Uprn.Should().Be("123456789012");
        result.LocalCustodianCode.Should().Be("123");
    }
    
    // This fictional example taken from page 41 of https://www.ordnancesurvey.co.uk/documents/product-support/getting-started/addressbase-addressbase-plus-addressbase-plus-islands-getting-started-guide.pdf
    [Test]
    public void Parse_WithOsExampleData_MatchesExampleString()
    {
        // Arrange
        var underTest = new OsPlacesLpiDto
        {
            Organisation = "JW SIMPSON LTD",
            SaoText = "THE ANNEXE",
            SaoStartNumber = "1",
            SaoStartSuffix = "A",
            PaoText = "THE OLD MILL",
            PaoStartNumber = "7",
            PaoEndNumber = "9",
            StreetDescription = "MAIN STREET",
            LocalityName = "HOOK",
            TownName = "WARSASH",
            AdministrativeArea = "SOUTHAMPTON",
            PostcodeLocator = "SO99 9ZZ",
            Uprn = "123456789012",
            LocalCustodianCode = "123"
        };
        
        // Act
        var result = underTest.Parse();
        
        // Assert
        result.AddressLine1.Should().Be("Jw Simpson Ltd, The Annexe, 1A, The Old Mill");
        result.AddressLine2.Should().Be("7–9, Main Street");
        result.Town.Should().Be("Warsash");
        result.Postcode.Should().Be("SO99 9ZZ");
        result.Uprn.Should().Be("123456789012");
        result.LocalCustodianCode.Should().Be("123");
    }
    
    [TestCase("C", "1", "RD06", true)] // Current residential postal address
    [TestCase("C", "1", "CE01", true)] // Current educational postal address
    [TestCase("C", "1", "X01", true)] // Current dual-use (residential and commercial) postal address
    [TestCase("C", "1", "M01", true)] // Current military postal address
    [TestCase("N", "1", "RD06", false)] // Current residential non-postal address
    [TestCase("C", "0", "RD06", false)] // Non-current residential postal address
    [TestCase("C", "1", "PP", false)] // Current non-residential/educational/etc postal address
    public void IsCurrentResidential_WithVariousData_ReturnsAsExpected(
        string postalAddressCode,
        string lpiLogicalStatusCode,
        string classificationCode,
        bool expectedResult)
    {
        // Arrange
        var underTest = new OsPlacesLpiDto
        {
            PostalAddressCode = postalAddressCode,
            LpiLogicalStatusCode = lpiLogicalStatusCode,
            ClassificationCode = classificationCode,
            PaoText = "EXAMPLE HOUSE",
            StreetDescription = "EXAMPLE ROAD",
            TownName = "EXAMPLETON",
            AdministrativeArea = "EXAMPLESHIRE",
            PostcodeLocator = "AB1 2CD",
            Uprn = "123456789012",
            LocalCustodianCode = "123",
        };
        
        // Act
        var result = underTest.IsCurrentResidential();
        
        // Assert
        result.Should().Be(expectedResult);
    }
}
