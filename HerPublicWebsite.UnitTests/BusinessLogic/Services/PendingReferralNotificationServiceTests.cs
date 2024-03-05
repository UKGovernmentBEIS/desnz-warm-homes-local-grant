using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using HerPublicWebsite.BusinessLogic;
using HerPublicWebsite.BusinessLogic.ExternalServices.EmailSending;
using HerPublicWebsite.BusinessLogic.Models;
using HerPublicWebsite.BusinessLogic.Services.CsvFileCreator;
using HerPublicWebsite.BusinessLogic.Services.RegularJobs;
using Moq;
using NUnit.Framework;

namespace Tests.BusinessLogic.Services;

[TestFixture]
public class PendingReferralNotificationServiceTests
{
    private Mock<IDataAccessProvider> mockDataAccessProvider;
    private Mock<CsvFileCreator> mockCsvFileCreator;
    private Mock<IEmailSender> mockEmailSender;
    private PendingReferralNotificationService pendingReferralNotificationService;
    
    [SetUp]
    public void Setup()
    {
        mockDataAccessProvider = new Mock<IDataAccessProvider>();
        mockCsvFileCreator = new Mock<CsvFileCreator>();
        mockEmailSender = new Mock<IEmailSender>();
        pendingReferralNotificationService = new PendingReferralNotificationService(
            mockDataAccessProvider.Object, 
            mockCsvFileCreator.Object, 
            mockEmailSender.Object);
    }

    [Test]
    public async Task SendPendingReferralNotifications_WhenCalled_CallsSendPendingReferralReportEmail()
    {
        // Arrange
        mockDataAccessProvider
            .Setup(dap => dap.GetPendingReferralRequestsBetweenDates(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(new List<ReferralRequest>());
        
        // Act
        await pendingReferralNotificationService.SendPendingReferralNotifications();
        
        // Assert
        mockEmailSender.Verify(es => es.SendPendingReferralReportEmail(It.IsAny<MemoryStream>()));
    }
}