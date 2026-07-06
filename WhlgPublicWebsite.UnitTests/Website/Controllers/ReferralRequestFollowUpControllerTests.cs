using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using WhlgPublicWebsite.BusinessLogic.Services.ReferralFollowUps;
using WhlgPublicWebsite.Controllers;
using WhlgPublicWebsite.Models.Enums;
using WhlgPublicWebsite.Models.ReferralRequestFollowUp;

namespace Tests.Website.Controllers;

[TestFixture]
public class ReferralRequestFollowUpControllerTests
{
    private Mock<IReferralFollowUpService> mockReferralFollowUpService;
    private ReferralRequestFollowUpController controller;

    [SetUp]
    public void Setup()
    {
        mockReferralFollowUpService = new Mock<IReferralFollowUpService>();
        controller = new ReferralRequestFollowUpController(mockReferralFollowUpService.Object);
    }

    [Test]
    public async Task RespondPage_Post_WhenResponseIsRecorded_RedirectsToResponseRecorded()
    {
        // Arrange
        var viewModel = new ReferralRequestFollowUpResponsePageViewModel
        {
            Token = "test-token",
            HasFollowedUp = YesOrNo.Yes
        };

        // Act
        var result = await controller.RespondPage_Post(viewModel);

        // Assert
        var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Which;
        redirectResult.ActionName.Should().Be(nameof(ReferralRequestFollowUpController.ResponseRecorded));
        redirectResult.ControllerName.Should().Be("ReferralRequestFollowUp");
        mockReferralFollowUpService.Verify(
            service => service.RecordFollowUpResponseForToken(viewModel.Token, true),
            Times.Once);
        mockReferralFollowUpService.VerifyNoOtherCalls();
    }

    [Test]
    public async Task RespondPage_Post_WhenTokenHasAlreadyBeenUsed_RedirectsToAlreadyResponded()
    {
        // Arrange
        var viewModel = new ReferralRequestFollowUpResponsePageViewModel
        {
            Token = "test-token",
            HasFollowedUp = YesOrNo.No
        };

        mockReferralFollowUpService
            .Setup(service => service.RecordFollowUpResponseForToken(viewModel.Token, false))
            .ThrowsAsync(new InvalidOperationException());

        // Act
        var result = await controller.RespondPage_Post(viewModel);

        // Assert
        var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Which;
        redirectResult.ActionName.Should().Be(nameof(ReferralRequestFollowUpController.AlreadyResponded));
        redirectResult.ControllerName.Should().Be("ReferralRequestFollowUp");
        mockReferralFollowUpService.Verify(
            service => service.RecordFollowUpResponseForToken(viewModel.Token, false),
            Times.Once);
        mockReferralFollowUpService.VerifyNoOtherCalls();
    }
}
