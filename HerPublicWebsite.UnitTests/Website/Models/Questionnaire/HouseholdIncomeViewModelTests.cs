using FluentAssertions;
using HerPublicWebsite.BusinessLogic.Models.Enums;
using HerPublicWebsite.Models.Questionnaire;
using NUnit.Framework;

namespace Tests.Website.Models.Questionnaire;

[TestFixture]
public class HouseholdIncomeViewModelTests
{
    [Test]
    public void IncomeBandOptions_ForMissingCustoidanCode_ReturnsNull()
    {
        // Arrange
        var underTest = new HouseholdIncomeViewModel()
        {
            CustodianCode = null
        };
        
        // Act
        var result = underTest.IncomeBandOptions;
        
        // Assert
        result.Should().BeNull();
    }
    
    [Test]
    public void IncomeBandOptions_ForCustoidanCode_ReturnsCorrectBands()
    {
        // Arrange
        var underTest = new HouseholdIncomeViewModel()
        {
            CustodianCode = "9052" // Aberdeenshire is configure with a £31,000 threshold and shouldn't change as it isn't taking part in HUG2
        };
        
        // Act
        var result = underTest.IncomeBandOptions;
        
        // Assert
        result.Should().BeEquivalentTo(new [] { IncomeBand.UnderOrEqualTo36000, IncomeBand.GreaterThan36000 });
    }
}