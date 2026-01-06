using System;
using System.Collections.Generic;
using System.Linq;
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
    private DateTime startOfPreviousMonth;
    private IReferralFilterService referralFilterService;

    [SetUp]
    public void Setup()
    {
        startOfPreviousMonth = 1.February(2024);
        referralFilterService = new ReferralFilterService();
    }

    // If LA is now pending, include.
    [TestCase(true, false, false)]
    [TestCase(true, false, true)]
    [TestCase(true, true, false)]
    [TestCase(true, true, true)]
    // If LA is not now pending but referral was submitted in the last month to a then pending LA, include.
    [TestCase(false, true, true)]
    public void WasSubmittedToPendingAuthority_WhenCalledWithEmailPendingReferral_IncludesInFilter(
        bool localAuthorityIsNowPending,
        bool localAuthorityWasPending,
        bool referralWasSubmittedInTheLastMonth)
    {
        // Arrange
        var referralRequest = BuildReferralRequest(localAuthorityIsNowPending, localAuthorityWasPending,
            referralWasSubmittedInTheLastMonth);
        
        // Act
        var referralSubmittedToPendingAuthority = referralFilterService.WasSubmittedToPendingAuthority(referralRequest, startOfPreviousMonth);

        // Assert
        referralSubmittedToPendingAuthority.Should().BeTrue();
    }

    [TestCase(false, false, false)]
    [TestCase(false, false, true)]
    [TestCase(false, true, false)]
    public void WasSubmittedToPendingAuthority_WhenCalledWithNotEmailPendingReferral_DoesNotIncludeInFilter(
        bool localAuthorityIsNowPending,
        bool localAuthorityWasPending,
        bool referralWasSubmittedInTheLastMonth)
    {
        // Arrange
        var referralRequest = BuildReferralRequest(localAuthorityIsNowPending, localAuthorityWasPending,
            referralWasSubmittedInTheLastMonth);

        // Act
        var referralSubmittedToPendingAuthority = referralFilterService.WasSubmittedToPendingAuthority(referralRequest, startOfPreviousMonth);

        // Assert
        referralSubmittedToPendingAuthority.Should().BeFalse();
    }

    [TestCase(true, false)]
    [TestCase(false, true)]
    public void WasSubmittedToNonPendingAuthority_WhenCalledOnReferral_ReturnsExpectedBool(bool wasSubmittedToPendingAuthority, bool expectedResult)
    {
        // Arrange
        var referralRequest = new ReferralRequestBuilder(10)
            .WithWasSubmittedToPendingLocalAuthority(wasSubmittedToPendingAuthority)
            .Build();

        // Act
        var referralSubmittedToNonPendingAuthority = referralFilterService.WasSubmittedToNonPendingAuthority(referralRequest);

        // Assert
        referralSubmittedToNonPendingAuthority.Should().Be(expectedResult);
    }

    [TestCase("test1@example.com", true)]
    [TestCase(null, false)]
    public void WasSubmittedWithContactEmailAddress_WhenCalledOnReferral_ReturnsExpectedBool(string emailAddress, bool expectedResult)
    {
        // Arrange
        var referralRequest = new ReferralRequestBuilder(10)
            .WithEmailAddress(emailAddress)
            .Build();

        // Act
        var referralHasEmailAddress = referralFilterService.WasSubmittedWithContactEmailAddress(referralRequest);

        // Assert
        referralHasEmailAddress.Should().Be(expectedResult);
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