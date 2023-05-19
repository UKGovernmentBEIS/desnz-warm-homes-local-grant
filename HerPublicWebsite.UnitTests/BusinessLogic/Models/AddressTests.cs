using FluentAssertions;
using HerPublicWebsite.BusinessLogic.Models;
using NUnit.Framework;

namespace Tests.BusinessLogic.Models;

[TestFixture]
public class AddresssTests
{
    [Test]
    public void DisplayAddress_WithAllFields_IncludesAllFields()
    {
        // Arrange
        var address = new Address()
        {
            AddressLine1 = "line 1",
            AddressLine2 = "line 2",
            Town = "town",
            County = "County",
            Postcode = "AB1 2CD"
        };
        
        // Act
        var result = address.DisplayAddress;
        
        // Assert
        result.Should().Be("line 1, line 2, town, County, AB1 2CD");
    }
    
    [Test]
    public void DisplayAddress_WithMissingLine2_SkipsLine2()
    {
        // Arrange
        var address = new Address()
        {
            AddressLine1 = "line 1",
            AddressLine2 = null,
            Town = "town",
            County = "County",
            Postcode = "AB1 2CD"
        };
        
        // Act
        var result = address.DisplayAddress;
        
        // Assert
        result.Should().Be("line 1, town, County, AB1 2CD");
    }
    
    [Test]
    public void DisplayAddress_WithMissingCounty_SkipsCounty()
    {
        // Arrange
        var address = new Address()
        {
            AddressLine1 = "line 1",
            AddressLine2 = "line 2",
            Town = "town",
            County = null,
            Postcode = "AB1 2CD"
        };
        
        // Act
        var result = address.DisplayAddress;
        
        // Assert
        result.Should().Be("line 1, line 2, town, AB1 2CD");
    }
}
