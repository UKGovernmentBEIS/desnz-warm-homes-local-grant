﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions.Extensions;
using HerPublicWebsite.BusinessLogic;
using HerPublicWebsite.BusinessLogic.ExternalServices.EmailSending;
using HerPublicWebsite.BusinessLogic.Models;
using HerPublicWebsite.BusinessLogic.Services.CsvFileCreator;
using HerPublicWebsite.BusinessLogic.Services.ReferralFollowUps;
using HerPublicWebsite.BusinessLogic.Services.RegularJobs;
using Moq;
using NUnit.Framework;
using Tests.Builders;
using Tests.Helpers;

namespace Tests.BusinessLogic.Services;

public class ReferralFollowUpNotificationServiceTests
{
    private GlobalConfiguration globalConfig;
    private ReferralRequestNotificationConfiguration requestNotificationConfig;
    private Mock<IDataAccessProvider> mockDataProvider;
    private Mock<ICsvFileCreator> mockCsvFileCreator;
    private Mock<IWorkingDayHelperService> mockWorkingDayHelperService;
    private Mock<IReferralFollowUpService> mockReferralFollowUpService;
    private Mock<IEmailSender> mockEmailSender;
    private Mock<IDateHelper> mockDateHelper;
    private IReferralFilterService referralFilterService;
    private ReferralFollowUpNotificationService referralFollowUpNotificationService;

    [SetUp]
    public void Setup()
    {
        globalConfig = new GlobalConfiguration
        {
            AppBaseUrl = "base-url"
        };
        requestNotificationConfig = new ReferralRequestNotificationConfiguration
        {
            CutoffEpoch = 1.January(2024)
        };
        mockDataProvider = new Mock<IDataAccessProvider>();
        mockCsvFileCreator = new Mock<ICsvFileCreator>();
        mockWorkingDayHelperService = new Mock<IWorkingDayHelperService>();
        mockReferralFollowUpService = new Mock<IReferralFollowUpService>();
        mockEmailSender = new Mock<IEmailSender>();
        mockDateHelper = new Mock<IDateHelper>();
        referralFilterService = new ReferralFilterService(mockDateHelper.Object);

        referralFollowUpNotificationService = new ReferralFollowUpNotificationService(
            globalConfig.AsOptions(),
            requestNotificationConfig.AsOptions(),
            mockEmailSender.Object,
            mockDataProvider.Object,
            mockCsvFileCreator.Object,
            mockWorkingDayHelperService.Object,
            mockReferralFollowUpService.Object,
            referralFilterService);
    }

    [Test]
    public async Task
        SendReferralFollowUpNotifications_WhenCalled_CallsCreateReferralsRequestFollowUpWithReferralsIncludedInFilter()
    {
        // Arrange
        var validReferral = new ReferralRequestBuilder(1)
            .WithWasSubmittedToPendingLocalAuthority(false)
            .Build();
        var invalidReferral = new ReferralRequestBuilder(2)
            .WithWasSubmittedToPendingLocalAuthority(true)
            .Build();

        var allReferrals = new List<ReferralRequest>
        {
            validReferral, invalidReferral
        };

        mockDataProvider
            .Setup(dp =>
                dp.GetHug2ReferralRequestsWithNoFollowUpBetweenDates(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(allReferrals);

        var referralFollowUp = new ReferralRequestFollowUpBuilder(1)
            .WithToken("token1")
            .Build(validReferral);

        mockReferralFollowUpService.Setup(rfus => rfus.CreateReferralRequestFollowUp(
            validReferral)).ReturnsAsync(referralFollowUp);

        // Act
        await referralFollowUpNotificationService.SendReferralFollowUpNotifications();

        // Assert
        mockReferralFollowUpService.Verify(rfus => rfus.CreateReferralRequestFollowUp(
            validReferral), Times.Once);
        mockReferralFollowUpService.VerifyNoOtherCalls();
    }
}