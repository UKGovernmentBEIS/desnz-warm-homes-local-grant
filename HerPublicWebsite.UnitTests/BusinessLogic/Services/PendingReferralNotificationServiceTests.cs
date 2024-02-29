using HerPublicWebsite.BusinessLogic.ExternalServices.EmailSending;
using HerPublicWebsite.BusinessLogic.Services.RegularJobs;
using Moq;
using NUnit.Framework;

namespace Tests.BusinessLogic.Services;

[TestFixture]
public class PendingReferralNotificationServiceTests
{
    private Mock<IEmailSender> mockEmailSender;
    private PendingReferralNotificationService pendingReferralNotificationService;
    
    [SetUp]
    public void Setup()
    {
        mockEmailSender = new Mock<IEmailSender>();
        pendingReferralNotificationService = new PendingReferralNotificationService(mockEmailSender.Object);
    }

    [Test]
    public void SendPendingReferralNotifications_WhenCalled_CallsSendPendingReferralReportEmail()
    {
        // Arrange
        
        // Act
        pendingReferralNotificationService.SendPendingReferralNotifications();
        
        // Assert
        mockEmailSender.Verify(es => es.SendPendingReferralReportEmail());
    }
}