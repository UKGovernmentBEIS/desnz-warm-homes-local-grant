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
        var underTest = new HouseholdIncomeViewModel
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
        var underTest = new HouseholdIncomeViewModel
        {
            CustodianCode = "9052" // Aberdeenshire is configured with a £36,000 threshold
        };

        // Act
        var result = underTest.IncomeBandOptions;

        // Assert
        result.Should().BeEquivalentTo(new[] { IncomeBand.UnderOrEqualTo36000, IncomeBand.GreaterThan36000 });
    }
}