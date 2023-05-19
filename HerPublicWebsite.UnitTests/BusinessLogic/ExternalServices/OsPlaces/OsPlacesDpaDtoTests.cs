using FluentAssertions;
using HerPublicWebsite.BusinessLogic.ExternalServices.OsPlaces;
using NUnit.Framework;

namespace Tests.BusinessLogic.ExternalServices.OsPlaces;

[TestFixture]
public class OsPlacesDpaDtoTests
{
    [Test]
    public void Parse_WithANormalAddress_PopulatesAddressDetails()
    {
        // Arrange
        var underTest = new OsPlacesDpaDto
        {
            BuildingNumber = "52A",
            ThoroughFareName = "A ROAD",
            PostTown = "ATOWN",
            Postcode = "AB1 2CD",
            Uprn = "123456789012",
            LocalCustodianCode = "123"
        };
        
        // Act
        var result = underTest.Parse();
        
        // Assert
        result.AddressLine1.Should().Be("52A A Road");
        result.AddressLine2.Should().BeNull();
        result.Town.Should().Be("Atown");
        result.Postcode.Should().Be("AB1 2CD");
        result.Uprn.Should().Be("123456789012");
        result.LocalCustodianCode.Should().Be("123");
    }
    
    [Test]
    public void Parse_WithADependentThoroughfare_MovesThoroughfareToLine2()
    {
        // Arrange
        var underTest = new OsPlacesDpaDto
        {
            BuildingNumber = "52A",
            DependentThoroughFareName = "DEPENDENT LANE",
            ThoroughFareName = "A ROAD",
            PostTown = "ATOWN",
            Postcode = "AB1 2CD",
            Uprn = "123456789012",
            LocalCustodianCode = "123"
        };
        
        // Act
        var result = underTest.Parse();
        
        // Assert
        result.AddressLine1.Should().Be("52A Dependent Lane");
        result.AddressLine2.Should().Be("A Road");
        result.Town.Should().Be("Atown");
        result.Postcode.Should().Be("AB1 2CD");
        result.Uprn.Should().Be("123456789012");
        result.LocalCustodianCode.Should().Be("123");
    }
}