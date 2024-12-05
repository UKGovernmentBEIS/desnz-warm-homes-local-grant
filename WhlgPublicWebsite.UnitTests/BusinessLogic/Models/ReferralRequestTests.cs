using System;
using FluentAssertions;
using NUnit.Framework;
using WhlgPublicWebsite.BusinessLogic.Models;
using Tests.Helpers;

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
    
    [TestCase(LocalAuthorityData.Hug2Status.Live, false)]
    [TestCase(LocalAuthorityData.Hug2Status.Pending, true)]
    public void WasSubmittedToPendingFlag_IfLocalAuthorityIsPending_WasSubmittedToPendingIsTrueOtherwiseFalse(LocalAuthorityData.Hug2Status localAuthorityStatus, bool expectedWasSubmittedToPendingLocalAuthorityValue)
    {
        // Arrange
        var testQuestionnaire = QuestionnaireHelper.InitializeQuestionnaire();
        testQuestionnaire.CustodianCode = LocalAuthorityDataHelper.GetExampleCustodianCodeForStatus(localAuthorityStatus);
        
        // Act
        var underTest = new ReferralRequest(testQuestionnaire);
        
        // Assert
        underTest.WasSubmittedToPendingLocalAuthority.Should().Be(expectedWasSubmittedToPendingLocalAuthorityValue);
    }
}
