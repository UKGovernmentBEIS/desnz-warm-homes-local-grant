using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Extensions;
using WhlgPublicWebsite.BusinessLogic;
using WhlgPublicWebsite.BusinessLogic.ExternalServices.EmailSending;
using WhlgPublicWebsite.BusinessLogic.Models;
using WhlgPublicWebsite.BusinessLogic.Services.CsvFileCreator;
using WhlgPublicWebsite.BusinessLogic.Services.PolicyTeamUpdate;
using WhlgPublicWebsite.BusinessLogic.Services.RegularJobs;
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
    private Mock<IDateHelper> mockDateHelper;
    private IReferralFilterService referralFilterService;
    private PolicyTeamUpdateService policyTeamUpdateService;

    [SetUp]
    public void Setup()
    {
        mockDataProvider = new Mock<IDataAccessProvider>();
        mockCsvFileCreator = new Mock<ICsvFileCreator>();
        mockWorkingDayHelperService = new Mock<IWorkingDayHelperService>();
        mockEmailSender = new Mock<IEmailSender>();
        mockDateHelper = new Mock<IDateHelper>();
        referralFilterService = new ReferralFilterService(mockDateHelper.Object);
        policyTeamUpdateService = new PolicyTeamUpdateService(
            mockDataProvider.Object,
            mockCsvFileCreator.Object,
            mockWorkingDayHelperService.Object,
            mockEmailSender.Object,
            referralFilterService);
    }

    [Test]
    public async Task SendPolicyUpdate_WhenCalled_CallsCsvFileCreatorWithReferralsIncludedInFilter()
    {
        // Arrange
        var validReferral = new ReferralRequestBuilder(1)
            .WithWasSubmittedToPendingLocalAuthority(false)
            .Build();
        var invalidReferral = new ReferralRequestBuilder(2)
            .WithWasSubmittedToPendingLocalAuthority(true)
            .Build();

        var allReferralsForReporting = new List<ReferralRequest>
        {
            validReferral, invalidReferral
        };
        var filteredReferrals = new List<ReferralRequest>
        {
            validReferral
        };

        mockDataProvider
            .Setup(dp =>
                dp.GetWhlgReferralRequestsBetweenDates(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(allReferralsForReporting);

        mockDataProvider
            .Setup(dp =>
                dp.GetAllWhlgReferralRequestsForSlaComplianceReporting())
            .ReturnsAsync(allReferralsForReporting);

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