using System;
using FluentAssertions;
using NUnit.Framework;
using WhlgPublicWebsite.BusinessLogic.Models;
using Tests.Helpers;

namespace Tests.BusinessLogic.Models;

[TestFixture]
public class ReferralRequestTests
{
    [TestCase(1, "WHLG0000001")] // First code
    [TestCase(9999999, "WHLG9999999")] // Highest expected code
    [TestCase(99999999, "WHLG99999999")] // Things work sensibly if we get too many referrals
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
    
    [TestCase(LocalAuthorityData.LocalAuthorityStatus.Live, false, Ignore = "PC-1828: Reinstate test after Live LA added")]
    [TestCase(LocalAuthorityData.LocalAuthorityStatus.Pending, true)]
    public void WasSubmittedToPendingFlag_IfLocalAuthorityIsPending_WasSubmittedToPendingIsTrueOtherwiseFalse(LocalAuthorityData.LocalAuthorityStatus localAuthorityStatus, bool expectedWasSubmittedToPendingLocalAuthorityValue)
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
