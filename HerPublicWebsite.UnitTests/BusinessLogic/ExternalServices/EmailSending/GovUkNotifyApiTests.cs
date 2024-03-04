using System;
using System.Collections.Generic;
using System.Linq;
using HerPublicWebsite.BusinessLogic.ExternalServices.EmailSending;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
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

    [SetUp]
    public void Setup()
    {
        logger = new NullLogger<GovUkNotifyApi>();
        mockNotificationClient = new Mock<INotificationClient>();
    }

    [Test]
    public void SendPendingReferralReportEmail_WhenCalled_CallsSendEmailOnEmailInConfig()
    {
        // Arrange
        const string exampleRecipient = "email1@example.com";
        const string exampleId = "0";
        const string exampleLink = "https://www.gov.uk/apply-home-upgrade-grant";
        config = new GovUkNotifyConfiguration
        {
            PendingReferralEmailRecipients = exampleRecipient,
            PendingReferralReportTemplate = new PendingReferralReportConfiguration
            {
                Id = exampleId,
                LinkPlaceholder = exampleLink
            }
        };
        var emailSender = new GovUkNotifyApi(mockNotificationClient.Object, config.AsOptions(), logger);
        
        // Act
        emailSender.SendPendingReferralReportEmail();
        
        // Assert
        mockNotificationClient.Verify(nc => nc.SendEmail(
            exampleRecipient,
            exampleId,
            new Dictionary<string, object>
            {
                { config.PendingReferralReportTemplate.LinkPlaceholder, exampleLink }
            },
            null, null));
    }

    [Test]
    public void SendPendingReferralReportEmail_WhenCalled_CallsSendEmailOnAllEmailsInConfig()
    {
        // Arrange
        string[] exampleRecipients = { "email1@example.com", "email2@example.com", "email3@example.com" };
        const string exampleId = "0";
        const string exampleLink = "https://www.gov.uk/apply-home-upgrade-grant";
        config = new GovUkNotifyConfiguration
        {
            PendingReferralEmailRecipients = String.Join(",", exampleRecipients),
            PendingReferralReportTemplate = new PendingReferralReportConfiguration
            {
                Id = exampleId,
                LinkPlaceholder = exampleLink
            }
        };
        var emailSender = new GovUkNotifyApi(mockNotificationClient.Object, config.AsOptions(), logger);
        
        // Act
        emailSender.SendPendingReferralReportEmail();
        
        // Assert
        mockNotificationClient.Verify(nc => nc.SendEmail(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<Dictionary<string, dynamic>>(),
            null, null),
            Times.Exactly(exampleRecipients.Length));
        
        foreach (var recipient in exampleRecipients)
        {
            mockNotificationClient.Verify(nc => nc.SendEmail(
                recipient,
                exampleId,
                new Dictionary<string, object>
                {
                    { config.PendingReferralReportTemplate.LinkPlaceholder, exampleLink }
                },
                null, null));
        }
    }
}