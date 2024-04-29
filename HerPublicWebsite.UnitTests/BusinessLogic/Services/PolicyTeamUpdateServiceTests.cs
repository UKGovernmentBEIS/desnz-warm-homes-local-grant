using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Extensions;
using HerPublicWebsite.BusinessLogic;
using HerPublicWebsite.BusinessLogic.ExternalServices.EmailSending;
using HerPublicWebsite.BusinessLogic.Models;
using HerPublicWebsite.BusinessLogic.Services.CsvFileCreator;
using HerPublicWebsite.BusinessLogic.Services.PolicyTeamUpdate;
using HerPublicWebsite.BusinessLogic.Services.RegularJobs;
using Moq;
using NUnit.Framework;
using Tests.Builders;

namespace Tests.BusinessLogic.Services;

public class PolicyTeamUpdateServiceTests
{
    private Mock<IDataAccessProvider> mockDataProvider;
    private Mock<ICsvFileCreator> mockCsvFileCreator;
    private Mock<IWorkingDayHelperService> mockWorkingDayHelperService;
    private Mock<IEmailSender> mockEmailSender;
    private Mock<IReferralFilterService> mockReferralFilterService;
    private PolicyTeamUpdateService policyTeamUpdateService;

    [SetUp]
    public void Setup()
    {
        mockDataProvider = new Mock<IDataAccessProvider>();
        mockCsvFileCreator = new Mock<ICsvFileCreator>();
        mockWorkingDayHelperService = new Mock<IWorkingDayHelperService>();
        mockEmailSender = new Mock<IEmailSender>();
        mockReferralFilterService = new Mock<IReferralFilterService>();
        policyTeamUpdateService = new PolicyTeamUpdateService(
            mockDataProvider.Object,
            mockCsvFileCreator.Object,
            mockWorkingDayHelperService.Object,
            mockEmailSender.Object,
            mockReferralFilterService.Object);
    }

    [Test]
    public async Task SendPolicyUpdate_WhenCalled_CallsCsvFileCreatorWithReferralsIncludedInFilter()
    {
        // Arrange
        var validReferral = new ReferralRequestBuilder(1)
            .Build();
        var invalidReferral = new ReferralRequestBuilder(2)
            .Build();
        
        var allReferrals = new List<ReferralRequest>()
        {
            validReferral, invalidReferral
        };
        var filteredReferrals = new List<ReferralRequest>()
        {
            validReferral
        };

        mockDataProvider
            .Setup(dp =>
                dp.GetReferralRequestsBetweenDates(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(allReferrals);

        mockDataProvider
            .Setup(dp =>
                dp.GetAllReferralRequests())
            .ReturnsAsync(allReferrals);

        mockReferralFilterService
            .Setup(rfs =>
                rfs.FilterForSentToNonPending(allReferrals))
            .Returns(filteredReferrals);
        
        // Act
        await policyTeamUpdateService.SendPolicyTeamUpdate();
        
        // Assert
        mockCsvFileCreator.Verify(cfc => cfc.CreateReferralRequestOverviewFileData(
            filteredReferrals), Times.Once);
        mockCsvFileCreator.Verify(cfc => cfc.CreateLocalAuthorityReferralRequestFollowUpFileData(
            filteredReferrals), Times.Exactly(2));
        mockCsvFileCreator.Verify(cfc => cfc.CreateConsortiumReferralRequestFollowUpFileData(
            filteredReferrals), Times.Exactly(2));
        mockCsvFileCreator.VerifyNoOtherCalls();
    }
}