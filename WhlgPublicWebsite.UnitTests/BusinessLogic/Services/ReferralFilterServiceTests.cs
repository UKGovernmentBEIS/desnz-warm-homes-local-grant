using System;
using System.Collections.Generic;
using FluentAssertions;
using FluentAssertions.Extensions;
using WhlgPublicWebsite.BusinessLogic.Models;
using WhlgPublicWebsite.BusinessLogic.Services.RegularJobs;
using Moq;
using NUnit.Framework;
using Tests.Builders;
using Tests.Helpers;
using static WhlgPublicWebsite.BusinessLogic.Models.LocalAuthorityData;

namespace Tests.BusinessLogic.Services;

public class ReferralFilterServiceTests
{
    private Mock<IDateHelper> mockDateHelper;
    private ReferralFilterService referralFilterService;
    private DateTime startOfPreviousMonth;

    [SetUp]
    public void Setup()
    {
        startOfPreviousMonth = 1.February(2024);
        mockDateHelper = new Mock<IDateHelper>();
        mockDateHelper.Setup(mdh => mdh.GetStartOfPreviousMonth()).Returns(startOfPreviousMonth);
        
        referralFilterService = new ReferralFilterService(mockDateHelper.Object);
    }

    // If LA is now pending, include.
    [TestCase(true, false, false)]
    [TestCase(true, false, true)]
    [TestCase(true, true, false)]
    [TestCase(true, true, true)]
    // If LA is not now pending but referral was submitted in the last month to a then pending LA, include.
    [TestCase(false, true, true)]
    public void FilterForPendingReferralReport_WhenCalledWithEmailPendingReferral_IncludesInFilter(
        bool localAuthorityIsNowPending,
        bool localAuthorityWasPending,
        bool referralWasSubmittedInTheLastMonth)
    {
        // Arrange
        var inputReferralRequest = BuildReferralRequest(localAuthorityIsNowPending, localAuthorityWasPending, referralWasSubmittedInTheLastMonth);
        var inputReferralRequests = new List<ReferralRequest> { inputReferralRequest };
        
        // Act
        var outputReferralRequests = referralFilterService
            .FilterForPendingReferralReport(inputReferralRequests);
        
        // Assert
        outputReferralRequests.Should().Contain(inputReferralRequest);
    }
    
    [TestCase(false, false, false)]
    [TestCase(false, false, true)]
    [TestCase(false, true, false)]
    public void FilterForPendingReferralReport_WhenCalledWithNotEmailPendingReferral_DoesNotIncludeInFilter(
        bool localAuthorityIsNowPending,
        bool localAuthorityWasPending,
        bool referralWasSubmittedInTheLastMonth)
    {
        // Arrange
        var inputReferralRequest = BuildReferralRequest(localAuthorityIsNowPending, localAuthorityWasPending, referralWasSubmittedInTheLastMonth);
        var inputReferralRequests = new List<ReferralRequest> { inputReferralRequest };
        
        // Act
        var outputReferralRequests = referralFilterService
            .FilterForPendingReferralReport(inputReferralRequests);
        
        // Assert
        outputReferralRequests.Should().NotContain(inputReferralRequest);
    }
    
    [Test]
    public void FilterForSentToNonPending_WhenCalledWithReferralSendToNonPendingLa_DoesIncludeInFilter()
    {
        // Arrange
        var inputReferralRequest = new ReferralRequestBuilder(10)
            .WithWasSubmittedToPendingLocalAuthority(false)
            .Build();
        var inputReferralRequests = new List<ReferralRequest> { inputReferralRequest };
        
        // Act
        var outputReferralRequests = referralFilterService
            .FilterForSentToNonPending(inputReferralRequests);
        
        // Assert
        outputReferralRequests.Should().Contain(inputReferralRequest);
    }
    
    [Test]
    public void FilterForSentToNonPending_WhenCalledWithReferralSendToPendingLa_DoesNotIncludeInFilter()
    {
        // Arrange
        var inputReferralRequest = new ReferralRequestBuilder(10)
            .WithWasSubmittedToPendingLocalAuthority(true)
            .Build();
        var inputReferralRequests = new List<ReferralRequest> { inputReferralRequest };
        
        // Act
        var outputReferralRequests = referralFilterService
            .FilterForSentToNonPending(inputReferralRequests);
        
        // Assert
        outputReferralRequests.Should().NotContain(inputReferralRequest);
    }

    private ReferralRequest BuildReferralRequest(
        bool localAuthorityIsNowPending,
        bool localAuthorityWasPending,
        bool referralWasSubmittedInTheLastMonth)
    {
        var currentStatus = localAuthorityIsNowPending ? LocalAuthorityStatus.Pending : LocalAuthorityStatus.Live;
        var custodianCode = LocalAuthorityDataHelper.GetExampleCustodianCodeForStatus(currentStatus);
        
        var requestDate = referralWasSubmittedInTheLastMonth
            ? startOfPreviousMonth.Add(10.Days())
            : startOfPreviousMonth.Subtract(10.Days());
        
        return new ReferralRequestBuilder(10)
            .WithCustodianCode(custodianCode)
            .WithWasSubmittedToPendingLocalAuthority(localAuthorityWasPending)
            .WithRequestDate(requestDate)
            .Build();
    }
}