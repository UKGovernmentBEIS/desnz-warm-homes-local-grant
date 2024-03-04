using System.Collections.Generic;
using HerPublicWebsite.BusinessLogic.ExternalServices.EmailSending;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Notify.Interfaces;
using NUnit.Framework;
using Tests.Helpers;

namespace Tests.BusinessLogic.ExternalServices.EmailSending;

[TestFixture]
public class GovUkNotifyApiTests
{
    private GovUkNotifyConfiguration config;
    private ILogger<GovUkNotifyApi> logger;
    private Mock<INotificationClient> mockNotificationClient;
    private GovUkNotifyApi govUkNotifyApi;

    [SetUp]
    public void Setup()
    {
        logger = new NullLogger<GovUkNotifyApi>();
        mockNotificationClient = new Mock<INotificationClient>();
        
        config = new GovUkNotifyConfiguration
        {
            PendingReferralReportTemplate = new PendingReferralReportConfiguration
            {
                Id = "test-pending-template-id",
                LinkPlaceholder = "link"
            }
        };
        govUkNotifyApi = new GovUkNotifyApi(mockNotificationClient.Object, config.AsOptions(), logger);
    }

    [Test]
    public void SendPendingReferralReportEmail_WhenCalled_CallsSendEmailOnEmailInConfig()
    {
        // Arrange
        const string recipient = "email1@example.com";
        config.PendingReferralEmailRecipients = recipient;
        
        // Act
        govUkNotifyApi.SendPendingReferralReportEmail();
        
        // Assert
        mockNotificationClient.Verify(nc => nc.SendEmail(
            recipient,
            config.PendingReferralReportTemplate.Id,
            new Dictionary<string, object>
            {
                { config.PendingReferralReportTemplate.LinkPlaceholder, "https://www.gov.uk/apply-home-upgrade-grant" }
            },
            null, null));
    }

    [Test]
    public void SendPendingReferralReportEmail_WhenCalled_CallsSendEmailOnAllEmailsInConfig()
    {
        // Arrange
        var recipients = new[] { "email1@example.com", "email2@example.com", "email3@example.com" };
        config.PendingReferralEmailRecipients = string.Join(",", recipients);
        
        // Act
        govUkNotifyApi.SendPendingReferralReportEmail();
        
        // Assert
        mockNotificationClient.Verify(nc => nc.SendEmail(
            It.IsAny<string>(),
            config.PendingReferralReportTemplate.Id,
            new Dictionary<string, object>
            {
                { config.PendingReferralReportTemplate.LinkPlaceholder, "https://www.gov.uk/apply-home-upgrade-grant" }
            },
            null, null),
            Times.Exactly(recipients.Length));
        
        foreach (var recipient in recipients)
        {
            mockNotificationClient.Verify(nc => nc.SendEmail(
                recipient,
                config.PendingReferralReportTemplate.Id,
                new Dictionary<string, object>
                {
                    { config.PendingReferralReportTemplate.LinkPlaceholder, "https://www.gov.uk/apply-home-upgrade-grant" }
                },
                null, null));
        }
    }

    [Test]
    public void SendPendingReferralReportEmail_WhenCalled_DoesNotCallSendEmailIfEmailConfigIsEmpty()
    {
        // Arrange
        const string recipient = "";
        config.PendingReferralEmailRecipients = recipient;
        
        // Act
        govUkNotifyApi.SendPendingReferralReportEmail();
        
        // Assert
        mockNotificationClient.Verify(nc => nc.SendEmail(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<Dictionary<string, dynamic>>(),
            null, null), Times.Never);
    }

    [Test]
    public void SendPendingReferralReportEmail_WhenCalled_DoesNotCallSendEmailIfEmailConfigIsNull()
    {
        // Arrange
        config.PendingReferralEmailRecipients = null;
        
        // Act
        govUkNotifyApi.SendPendingReferralReportEmail();
        
        // Assert
        mockNotificationClient.Verify(nc => nc.SendEmail(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<Dictionary<string, dynamic>>(),
            null, null), Times.Never);
    }
}