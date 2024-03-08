using System.Collections.Generic;
using HerPublicWebsite.BusinessLogic.Models;
using HerPublicWebsite.BusinessLogic.Services.RegularJobs;
using Moq;
using NUnit.Framework;
using Tests.Builders;
using Tests.Helpers;
using DateTime = System.DateTime;

namespace Tests.BusinessLogic.Services;

public class PendingReferralFilterServiceTests
{
    private Mock<IDateHelper> mockDateHelper;
    private PendingReferralFilterService pendingReferralFilterService;
    private ReferralRequest inputReferralRequest;

    [SetUp]
    public void Setup()
    {
        mockDateHelper = new Mock<IDateHelper>();
        pendingReferralFilterService = new PendingReferralFilterService(mockDateHelper.Object);
    }

    [TestCase(false, true, true)] // case 2
    [TestCase(true, false, false)] // case 1
    [TestCase(true, false, true)] // case 1, 3
    [TestCase(true, true, false)] // case 1
    [TestCase(true, true, true)] // case 1
    public void PendingReferralFilterService_WhenCalledWithEmailPendingReferral_IncludesInFilter(
        bool localAuthorityIsNowPending,
        bool localAuthorityWasPending,
        bool referralWasSubmittedInTheLastMonth)
    {
        SetupReferralData(localAuthorityIsNowPending, localAuthorityWasPending, referralWasSubmittedInTheLastMonth);
        var inputReferralRequests = new List<ReferralRequest>() { inputReferralRequest };
        
        // Act
        var outputReferralRequests = pendingReferralFilterService
            .FilterForPendingReferralReport(inputReferralRequests);
        
        // Assert
        CollectionAssert.Contains(outputReferralRequests, inputReferralRequest);
    }

    [TestCase(false, false, false)]
    [TestCase(false, false, true)]
    [TestCase(false, true, false)]
    public void PendingReferralFilterService_WhenCalledWithNotEmailPendingReferral_DoesNotIncludeInFilter(
        bool localAuthorityIsNowPending,
        bool localAuthorityWasPending,
        bool referralWasSubmittedInTheLastMonth)
    {
        SetupReferralData(localAuthorityIsNowPending, localAuthorityWasPending, referralWasSubmittedInTheLastMonth);
        var inputReferralRequests = new List<ReferralRequest>() { inputReferralRequest };
        
        // Act
        var outputReferralRequests = pendingReferralFilterService
            .FilterForPendingReferralReport(inputReferralRequests);
        
        // Assert
        CollectionAssert.DoesNotContain(outputReferralRequests, inputReferralRequest);
    }

    private void SetupReferralData(
        bool localAuthorityIsNowPending,
        bool localAuthorityWasPending,
        bool referralWasSubmittedInTheLastMonth)
    {
        // Arrange
        var custodianCode =
            LocalAuthorityDataHelper.GetExampleCustodianCodeForStatus(
                localAuthorityIsNowPending
                    ? LocalAuthorityData.Hug2Status.Pending
                    : LocalAuthorityData.Hug2Status.Live);

        var requestDate = new DateTime(
            2024,
            referralWasSubmittedInTheLastMonth ? 2 : 1, // as 'last month' is defined as 1st february
            15);
        inputReferralRequest = new ReferralRequestBuilder(10)
            .WithCustodianCode(custodianCode)
            .WithWasSubmittedToPendingLocalAuthority(localAuthorityWasPending)
            .WithRequestDate(requestDate)
            .Build();

        var oneMonthAgoDate = new DateTime(2024, 2, 1);
        
        mockDateHelper.Setup(mdh => mdh.GetStartOfPreviousMonth()).Returns(oneMonthAgoDate);
    }
}