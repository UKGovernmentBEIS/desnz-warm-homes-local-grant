using System.Linq;
using FluentAssertions;
using WhlgPublicWebsite.BusinessLogic.Models;
using WhlgPublicWebsite.Models.Questionnaire;
using NUnit.Framework;

namespace Tests.Website.Models.Questionnaire;

[TestFixture]
public class SelectLocalAuthorityViewModelTests
{
    [Test]
    public void LocalAuthoritiesByInitial_WithNoSearch_ReturnsAllLocalAuthorities()
    {
        // Arrange
        var underTest = new SelectLocalAuthorityViewModel()
        {
            SearchTerm = null
        };
        
        // Act
        var result = underTest.LocalAuthoritiesByInitial;
        
        // Assert
        result.SelectMany(r => r.Value).Count().Should()
            .Be(LocalAuthorityData.LocalAuthorityDetailsByCustodianCode.Count);
    }
    
    [Test]
    public void LocalAuthoritiesByInitial_WithSearch_FiltersLocalAuthorities()
    {
        // Arrange
        var underTest = new SelectLocalAuthorityViewModel()
        {
            SearchTerm = "Cambridge"
        };
        
        // Act
        var result = underTest.LocalAuthoritiesByInitial;
        
        // Assert
        result.Keys.ToList().Should().BeEquivalentTo(new[] {"C", "E", "S" });
        result
            .SelectMany(r => r.Value.Select(lad => lad.Name))
            .ToList()
            .Should()
            .BeEquivalentTo(new[]
            {
                "Cambridge City Council",
                "East Cambridgeshire District Council",
                "South Cambridgeshire District Council"
            });
    }
}