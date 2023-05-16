using System;
using FluentAssertions;
using NUnit.Framework;
using HerPublicWebsite.BusinessLogic.Models;
using HerPublicWebsite.BusinessLogic.Models.Enums;

namespace Tests.BusinessLogic.Models;

[TestFixture]
public class ReferralRequestTests
{
    [TestCase(1, "HUG20000001")] // First code
    [TestCase(9999999, "HUG29999999")] // Highest expected code
    [TestCase(99999999, "HUG299999999")] // Things work sensibly if we get too many referrals
    public void ReferralCode_IsGeneratedFromId(int id, string expectedReferralCode)
    {
        // Arrange
        var underTest = new ReferralRequest { Id = id };
        
        // Act
        underTest.UpdateReferralCode();

        // Assert
        underTest.ReferralCode.Should().Be(expectedReferralCode);
    }
    
    [Test]
    public void ReferralCode_CreatedWithNoId_ThrowsException()
    {
        // Arrange
        var underTest = new ReferralRequest();
        
        // Act
        var act = () => underTest.UpdateReferralCode();

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Test]
    public void ReferralRequest_CreatedWithIncorrectEpc_IgnoresEpcBand()
    {
        // Arrange
        var questionnaire = new Questionnaire()
        {
            EpcDetailsAreCorrect = false,
            EpcDetails = new EpcDetails
            {
                EpcRating = EpcRating.A
            },
            HasGasBoiler = HasGasBoiler.No,
            IncomeBand = IncomeBand.Under31000,
            IsLsoaProperty = false,
        };
        
        // Act
        var underTest = new ReferralRequest(questionnaire, null);
        
        // Assert
        underTest.EpcRating.Should().Be(EpcRating.Unknown);
    }
    
    [Test]
    public void ReferralRequest_CreatedWithCorrectEpc_RetainsEpcBand()
    {
        // Arrange
        var questionnaire = new Questionnaire()
        {
            EpcDetailsAreCorrect = true,
            EpcDetails = new EpcDetails
            {
                EpcRating = EpcRating.A
            },
            HasGasBoiler = HasGasBoiler.No,
            IncomeBand = IncomeBand.Under31000,
            IsLsoaProperty = false,
        };
        
        // Act
        var underTest = new ReferralRequest(questionnaire, null);
        
        // Assert
        underTest.EpcRating.Should().Be(EpcRating.A);
    }
}
