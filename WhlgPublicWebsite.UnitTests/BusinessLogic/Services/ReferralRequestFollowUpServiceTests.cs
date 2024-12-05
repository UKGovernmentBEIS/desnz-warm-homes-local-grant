using System;
using System.Threading.Tasks;
using FluentAssertions;
using HerPublicWebsite.BusinessLogic;
using HerPublicWebsite.BusinessLogic.Models;
using NUnit.Framework;
using Moq;
using Tests.Builders;
using HerPublicWebsite.BusinessLogic.Services.ReferralFollowUps;

namespace Tests.BusinessLogic.Services;

[TestFixture]
public class ReferralRequestFollowUpServiceTests
{
    private IReferralFollowUpService referralFollowUpService;
    private Mock<IDataAccessProvider> mockDataAccessProvider;
    private Mock<IGuidService> mockGuidService;

    [SetUp]
    public void Setup()
    {
        mockDataAccessProvider = new Mock<IDataAccessProvider>();
        mockGuidService = new Mock<IGuidService>();
        referralFollowUpService = new ReferralFollowUpService(mockDataAccessProvider.Object, mockGuidService.Object);
    }

    [Test]
    public async Task CreateReferralRequestFollowUp_WhenCalledWithNewReferral_CreatesANewReferralRequestFollowUpInTheDb()
    {
        // Arrange
        string testToken = "testToken";
        var newReferralRequest = new ReferralRequestBuilder(1).WithReferralCreated(false).WithRequestDate(new DateTime(2023, 03, 01)).Build();
        var newReferralRequestFollowUp = new ReferralRequestFollowUp(newReferralRequest, testToken);

        mockGuidService.Setup(gs => gs.NewGuidString()).Returns(testToken);
        mockDataAccessProvider.Setup(dap => dap.PersistReferralFollowUpToken(newReferralRequestFollowUp).Result).Returns(newReferralRequestFollowUp);
     
        // Act
        var referralRequestFollowUp = await referralFollowUpService.CreateReferralRequestFollowUp(newReferralRequest);

        // Assert
        mockDataAccessProvider.Verify(dap => dap.PersistReferralFollowUpToken(It.Is<ReferralRequestFollowUp>(rrfu => rrfu.Token == newReferralRequestFollowUp.Token && rrfu.ReferralRequest == newReferralRequestFollowUp.ReferralRequest)));
        referralRequestFollowUp.Should().BeEquivalentTo(newReferralRequestFollowUp);
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public async Task RecordFollowUpResponseForToken_WhenCalledWithTokenWhereAReferralRequestHasNotBeenFollowedUp_UpdatesReferralRequestFollowUp(bool hasFollowedUp)
    {
        // Arrange
        string testToken = "testToken";
        var newReferralRequest = new ReferralRequestBuilder(1).WithReferralCreated(false).WithRequestDate(new DateTime(2023, 03, 01)).Build();
        var newReferralRequestFollowUp = new ReferralRequestFollowUp(newReferralRequest, testToken);

        mockDataAccessProvider.Setup(dap => dap.GetReferralFollowUpByToken(testToken)).ReturnsAsync(newReferralRequestFollowUp);
        
        // Act
        await referralFollowUpService.RecordFollowUpResponseForToken(testToken, hasFollowedUp);

        // Assert
        mockDataAccessProvider.Verify(dap => dap.UpdateReferralFollowUpByTokenWithWasFollowedUp(testToken, hasFollowedUp));
    }

    [Test]
    public void RecordFollowUpResponseForToken_WhenCalledWithTokenWhereAReferralRequestHasAlreadyBeenFollowedUp_ThrowsInvalidOperationException()
    {
        // Arrange
        string testToken = "testToken";
        var newReferralRequest = new ReferralRequestBuilder(1).WithReferralCreated(false).WithRequestDate(new DateTime(2023, 03, 01)).Build();
        var newReferralRequestFollowUp = new ReferralRequestFollowUp(newReferralRequest, testToken)
        {
            WasFollowedUp = true
        };

        mockDataAccessProvider.Setup(dap => dap.GetReferralFollowUpByToken(testToken)).ReturnsAsync(newReferralRequestFollowUp);
        
        // Act
        // Assert
        Assert.ThrowsAsync<InvalidOperationException>(() => referralFollowUpService.RecordFollowUpResponseForToken(testToken, true));
     }
}
