using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using WhlgPublicWebsite.BusinessLogic.ExternalServices.EmailSending;
using WhlgPublicWebsite.BusinessLogic.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Notify.Interfaces;
using NUnit.Framework;
using Tests.Builders;
using Tests.Helpers;

namespace Tests.BusinessLogic.ExternalServices.EmailSending;

[TestFixture]
public class GovUkNotifyApiTests
{
    private GovUkNotifyConfiguration config;
    private ILogger<GovUkNotifyApi> logger;
    private Mock<INotificationClient> mockNotificationClient;
    private GovUkNotifyApi govUkNotifyApi;
    private MemoryStream memoryStream;

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
            },
            ReferralFollowUpTemplate = new ReferralFollowUpConfiguration
            {
                Id = "test-referral-follow-up-template-id",
                FollowUpLinkPlaceholder = "link",
                LocalAuthorityNamePlaceholder = "TestLA",
                RecipientNamePlaceholder = "TestRecipientName",
                ReferenceCodePlaceholder = "TestReferenceCode",
                ReferralDatePlaceholder = "TestReferralDate"
            },
            ReferenceCodeForLiveLocalAuthorityTemplate = new LiveReferenceCodeConfiguration
            {
                Id = "test-live-reference-code-template-id",
                RecipientNamePlaceholder = "TestName",
                ReferenceCodePlaceholder = "TestReferenceCode",
                TitleDeliveryPartnerPlaceholder = "TestTitleDeliveryPartner",
                TitleDeliveryPartnerOrContractorPlaceholder = "TestTitleDeliveryPartnerOrContractor",
                YourDeliveryPartnerOrContractorPlaceholder = "TestYourDeliveryPartnerOrContractor",
                WebsiteNamePlaceholder = "TestWebsiteName",
                WebsiteUrlPlaceholder = "TestWebsiteUrl"
            }
        };
        govUkNotifyApi = new GovUkNotifyApi(mockNotificationClient.Object, config.AsOptions(), logger);
        memoryStream = new MemoryStream("csv data"u8.ToArray());
    }

    [Test]
    public void SendPendingReferralReportEmail_WhenCalled_CallsSendEmailOnEmailInConfig()
    {
        // Arrange
        const string recipient = "email1@example.com";
        config.PendingReferralEmailRecipients = recipient;

        // Act
        govUkNotifyApi.SendPendingReferralReportEmail(memoryStream);

        // Assert
        mockNotificationClient.Verify(nc => nc.SendEmail(
            recipient,
            config.PendingReferralReportTemplate.Id,
            It.IsAny<Dictionary<string, object>>(),
            null, null, null));
    }

    [Test]
    public void SendPendingReferralReportEmail_WhenCalled_IncludesALinkInPersonalisation()
    {
        // Arrange
        const string recipient = "email1@example.com";
        config.PendingReferralEmailRecipients = recipient;

        // Act
        govUkNotifyApi.SendPendingReferralReportEmail(memoryStream);

        // Assert
        var personalisation = (Dictionary<string, object>)mockNotificationClient.Invocations[0].Arguments[2];
        personalisation.Should().ContainKey("link");
    }

    [Test]
    public void SendPendingReferralReportEmail_WhenCalled_CallsSendEmailOnAllEmailsInConfig()
    {
        // Arrange
        var recipients = new[] { "email1@example.com", "email2@example.com", "email3@example.com" };
        config.PendingReferralEmailRecipients = string.Join(",", recipients);

        // Act
        govUkNotifyApi.SendPendingReferralReportEmail(memoryStream);

        // Assert
        mockNotificationClient.Verify(nc => nc.SendEmail(
                It.IsAny<string>(),
                config.PendingReferralReportTemplate.Id,
                It.IsAny<Dictionary<string, object>>(),
                null, null, null),
            Times.Exactly(recipients.Length));

        foreach (var recipient in recipients)
        {
            mockNotificationClient.Verify(nc => nc.SendEmail(
                recipient,
                config.PendingReferralReportTemplate.Id,
                It.IsAny<Dictionary<string, object>>(),
                null, null, null));
        }
    }

    [Test]
    public void SendPendingReferralReportEmail_WhenCalled_DoesNotCallSendEmailIfEmailConfigIsEmpty()
    {
        // Arrange
        const string recipient = "";
        config.PendingReferralEmailRecipients = recipient;

        // Act
        govUkNotifyApi.SendPendingReferralReportEmail(memoryStream);

        // Assert
        mockNotificationClient.Verify(nc => nc.SendEmail(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<Dictionary<string, dynamic>>(),
            null, null, null), Times.Never);
    }

    [Test]
    public void SendPendingReferralReportEmail_WhenCalled_DoesNotCallSendEmailIfEmailConfigIsNull()
    {
        // Arrange
        config.PendingReferralEmailRecipients = null;

        // Act
        govUkNotifyApi.SendPendingReferralReportEmail(memoryStream);

        // Assert
        mockNotificationClient.Verify(nc => nc.SendEmail(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<Dictionary<string, dynamic>>(),
            null, null, null), Times.Never);
    }

    [TestCase(1, 10, 2022, "01/10/2022")]
    [TestCase(11, 1, 2023, "11/01/2023")]
    [TestCase(11, 12, 2023, "11/12/2023")]
    public void SendFollowUpEmail_WhenCalled_SendsEmailWithUkDateFormat(int day, int month, int year,
        string expectedDateString)
    {
        // Arrange
        var referralRequestBuilder = new ReferralRequestBuilder(1)
            .WithRequestDate(new DateTime(year, month, day))
            .WithCustodianCode(
                LocalAuthorityDataHelper.GetExampleCustodianCodeForStatus(LocalAuthorityData.LocalAuthorityStatus
                    .Live));
        var testReferralRequest = referralRequestBuilder.Build();
        var expectedKeyValuePair =
            new KeyValuePair<string, object>("TestReferralDate", expectedDateString);

        // Act
        govUkNotifyApi.SendFollowUpEmail(testReferralRequest, "example");

        // Assert
        mockNotificationClient.Verify(nc => nc.SendEmail(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.Is<Dictionary<string, dynamic>>(contents => contents.Contains(expectedKeyValuePair)),
            null, null, null), Times.Once);
    }

    [Test]
    public void SendLiveReferralReportEmail_WhenCalled_IncludesCorrectPersonalisation()
    {
        // Arrange
        var custodianCode = LocalAuthorityDataHelper.GetExampleCustodianCodeForStatusAndConsortium(LocalAuthorityData
            .LocalAuthorityStatus.Live, null); // LA not in a consortium
        var localAuthorityDetails = LocalAuthorityData.LocalAuthorityDetailsByCustodianCode[custodianCode];
        List<KeyValuePair<string, object>> expectedPersonalisations =
        [
            new("TestTitleDeliveryPartner", localAuthorityDetails.Name),
            new("TestTitleDeliveryPartnerOrContractor", $"{localAuthorityDetails.Name} or their official contractor"),
            new("TestYourDeliveryPartnerOrContractor", "Your Local Authority or their official contractor"),
            new("TestWebsiteName", $"{localAuthorityDetails.Name} website"),
            new("TestWebsiteUrl", localAuthorityDetails.WebsiteUrl)
        ];

        TestSendReferenceCodeEmailContainsPersonalisations(custodianCode, expectedPersonalisations);
    }

    [Test]
    public void SendLiveReferralReportEmail_WhenCalledForPortsmouthLa_IncludesCorrectPersonalisation()
    {
        // Arrange
        var custodianCode = LocalAuthorityDataHelper.GetExampleCustodianCodeForStatusAndConsortium(LocalAuthorityData
            .LocalAuthorityStatus.Live, ConsortiumNames.PortsmouthCityCouncil);
        var localAuthorityDetails = LocalAuthorityData.LocalAuthorityDetailsByCustodianCode[custodianCode];
        List<KeyValuePair<string, object>> expectedPersonalisations =
        [
            new("TestTitleDeliveryPartner", $"{localAuthorityDetails.Name}'s official delivery partner: Warmer Homes"),
            new("TestTitleDeliveryPartnerOrContractor",
                $"{localAuthorityDetails.Name}'s official delivery partner: Warmer Homes"),
            new("TestYourDeliveryPartnerOrContractor",
                $"{localAuthorityDetails.Name}'s official delivery partner: Warmer Homes"),
            new("TestWebsiteName", "Warmer Homes website"),
            new("TestWebsiteUrl", "https://www.warmerhomes.org.uk")
        ];

        TestSendReferenceCodeEmailContainsPersonalisations(custodianCode, expectedPersonalisations);
    }

    private void TestSendReferenceCodeEmailContainsPersonalisations(string custodianCode,
        List<KeyValuePair<string, object>> expectedPersonalisations)
    {
        var referralRequestBuilder = new ReferralRequestBuilder(1).WithCustodianCode(custodianCode);
        var testReferralRequest = referralRequestBuilder.Build();

        // Act
        govUkNotifyApi.SendReferenceCodeEmailForLiveLocalAuthority("email1@example.com", "example",
            testReferralRequest);

        // Assert
        mockNotificationClient.Verify(nc => nc.SendEmail(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<Dictionary<string, dynamic>>(),
            null, null, null), Times.Once);

        var personalisation = (Dictionary<string, object>)mockNotificationClient.Invocations[0].Arguments[2];

        foreach (var expectedKeyValuePair in expectedPersonalisations)
        {
            personalisation.Should().Contain(expectedKeyValuePair);
        }
    }
}