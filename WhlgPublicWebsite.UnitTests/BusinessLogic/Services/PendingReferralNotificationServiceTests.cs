using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WhlgPublicWebsite.BusinessLogic;
using WhlgPublicWebsite.BusinessLogic.ExternalServices.EmailSending;
using WhlgPublicWebsite.BusinessLogic.Models;
using WhlgPublicWebsite.BusinessLogic.Services.CsvFileCreator;
using WhlgPublicWebsite.BusinessLogic.Services.RegularJobs;
using Moq;
using NUnit.Framework;

namespace Tests.BusinessLogic.Services;

[TestFixture]
public class PendingReferralNotificationServiceTests
{
    private Mock<IDataAccessProvider> mockDataAccessProvider;
    private Mock<ICsvFileCreator> mockCsvFileCreator;
    private Mock<IEmailSender> mockEmailSender;
    private Mock<IReferralFilterService> mockReferralFilterService;
    private PendingReferralNotificationService pendingReferralNotificationService;

    [SetUp]
    public void Setup()
    {
        mockDataAccessProvider = new Mock<IDataAccessProvider>();
        mockCsvFileCreator = new Mock<ICsvFileCreator>();
        mockEmailSender = new Mock<IEmailSender>();
        mockReferralFilterService = new Mock<IReferralFilterService>();
        pendingReferralNotificationService = new PendingReferralNotificationService(
            mockDataAccessProvider.Object,
            mockCsvFileCreator.Object,
            mockEmailSender.Object,
            mockReferralFilterService.Object);
    }

    [Test]
    public async Task SendPendingReferralNotifications_WhenCalled_CallsSendPendingReferralReportEmail()
    {
        // Arrange
        var allReferralRequests = new List<ReferralRequest>();
        mockDataAccessProvider
            .Setup(dap => dap.GetAllWhlgReferralRequests())
            .ReturnsAsync(allReferralRequests);

        var filteredReferralRequests = new List<ReferralRequest>();
        mockReferralFilterService
            .Setup(prfs => prfs.FilterForPendingReferralReport(allReferralRequests))
            .Returns(filteredReferralRequests);

        var bytes = new byte[] { 0x0, 0x1, 0x2, 0x3 };
        var memoryStream = new MemoryStream(bytes);
        mockCsvFileCreator
            .Setup(cfc => cfc.CreatePendingReferralRequestFileDataForS3(filteredReferralRequests))
            .Returns(memoryStream);

        // Act
        await pendingReferralNotificationService.SendPendingReferralNotifications();

        // Assert
        mockEmailSender.Verify(es => es.SendPendingReferralReportEmail(memoryStream));
    }
}