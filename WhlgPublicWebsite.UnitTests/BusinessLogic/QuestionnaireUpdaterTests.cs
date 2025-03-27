using System;
using System.Threading.Tasks;
using FluentAssertions;
using WhlgPublicWebsite.BusinessLogic;
using WhlgPublicWebsite.BusinessLogic.ExternalServices.EmailSending;
using WhlgPublicWebsite.BusinessLogic.ExternalServices.EpbEpc;
using WhlgPublicWebsite.BusinessLogic.Models;
using WhlgPublicWebsite.BusinessLogic.Models.Enums;
using WhlgPublicWebsite.BusinessLogic.Services.EligiblePostcode;
using WhlgPublicWebsite.BusinessLogic.Services.QuestionFlow;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Tests.Builders;
using Tests.Helpers;

namespace Tests.BusinessLogic;

[TestFixture]
public class QuestionnaireUpdaterTests
{
    private QuestionnaireUpdater underTest;
    private Mock<IEpcApi> mockEpcApi;
    private Mock<IEligiblePostcodeService> mockPostCodeService;
    private Mock<IDataAccessProvider> mockDataAccessProvider;
    private Mock<IEmailSender> mockEmailSender;
    private Mock<IQuestionFlowService> mockQuestionFlowService;
    private Mock<ILogger<QuestionnaireUpdater>> mockLogger;
    private string liveCustodianCode;
    private string pendingCustodianCode;
    private string takingFutureReferralsCustodianCode;

    [SetUp]
    public void Setup()
    {
        // TODO: PC-1828: Reinstate test after Live LA added
        // liveCustodianCode =
        //     LocalAuthorityDataHelper.GetExampleCustodianCodeForStatus(LocalAuthorityData.LocalAuthorityStatus.Live);
        pendingCustodianCode =
            LocalAuthorityDataHelper.GetExampleCustodianCodeForStatus(LocalAuthorityData.LocalAuthorityStatus.Pending);
        // takingFutureReferralsCustodianCode =
        //     LocalAuthorityDataHelper.GetExampleCustodianCodeForStatus(LocalAuthorityData.LocalAuthorityStatus
        //         .TakingFutureReferrals);
        mockEpcApi = new Mock<IEpcApi>();
        mockPostCodeService = new Mock<IEligiblePostcodeService>();
        mockDataAccessProvider = new Mock<IDataAccessProvider>();
        mockEmailSender = new Mock<IEmailSender>();
        mockQuestionFlowService = new Mock<IQuestionFlowService>();
        mockLogger = new Mock<ILogger<QuestionnaireUpdater>>();
        underTest = new QuestionnaireUpdater
        (
            mockEpcApi.Object,
            mockPostCodeService.Object,
            mockDataAccessProvider.Object,
            mockEmailSender.Object,
            mockQuestionFlowService.Object,
            mockLogger.Object
        );
    }

    [Test]
    public async Task UpdateAddressAsync_CalledWithUprn_GetsEpcDetails()
    {
        // Arrange
        var questionnaire = new Questionnaire();
        var address = new Address()
        {
            AddressLine1 = "line1",
            County = "county",
            Postcode = "ab1 2cd",
            Uprn = "123456789012"
        };
        var epcDetails = new EpcDetails()
        {
            AddressLine1 = "epc line 1",
        };
        mockEpcApi.Setup(e => e.EpcFromUprnAsync("123456789012")).ReturnsAsync(epcDetails);

        // Act
        var result = await underTest.UpdateAddressAsync(questionnaire, address, null);

        // Assert
        mockEpcApi.Verify(e => e.EpcFromUprnAsync("123456789012"));
        result.EpcDetails.Should().Be(epcDetails);
        result.EpcDetailsAreCorrect.Should().BeNull();
    }

    [Test]
    public async Task UpdateAddressAsync_CalledWithoutUprn_ResetsEpcDetails()
    {
        // Arrange
        var questionnaire = new Questionnaire
        {
            EpcDetails = new EpcDetails()
        };
        var address = new Address()
        {
            AddressLine1 = "line1",
            County = "county",
            Postcode = "ab1 2cd"
        };

        // Act
        var result = await underTest.UpdateAddressAsync(questionnaire, address, null);

        // Assert
        mockEpcApi.VerifyNoOtherCalls();
        result.EpcDetails.Should().BeNull();
        result.EpcDetailsAreCorrect.Should().BeNull();
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task UpdateAddressAsync_WhenCalled_SetsLsoaStatusToMatchEligibility(bool isEligible)
    {
        // Arrange
        var postcode = "ab1 2cd";
        var questionnaire = new Questionnaire();
        var address = new Address()
        {
            AddressLine1 = "line1",
            County = "county",
            Postcode = postcode
        };
        mockPostCodeService.Setup(pcs => pcs.IsEligiblePostcode(postcode)).Returns(isEligible);

        // Act
        var result = await underTest.UpdateAddressAsync(questionnaire, address, null);

        // Assert
        result.IsLsoaProperty.Should().Be(isEligible);
    }

    [Test]
    public void UpdateLocalAuthority_WhenCalled_ResetsLocalAuthorityConfirmed()
    {
        // Arrange
        var questionnaire = new Questionnaire()
        {
            CustodianCode = "old code",
            LocalAuthorityConfirmed = true
        };

        // Act
        var result = underTest.UpdateLocalAuthority(questionnaire, "new code", null);

        // Assert
        result.LocalAuthorityConfirmed.Should().BeNull();
        result.CustodianCode.Should().Be("new code");
    }

    [Test]
    public async Task GenerateReferralAsync_WhenCalled_PersistReferral()
    {
        // Arrange
        var questionnaire = new Questionnaire
        {
            IsLsoaProperty = false,
            IncomeBand = IncomeBand.UnderOrEqualTo36000
        };
        mockDataAccessProvider.Setup(dap =>
            dap.PersistNewReferralRequestAsync(It.IsAny<ReferralRequest>())).ReturnsAsync(new ReferralRequest());

        // Act
        await underTest.GenerateReferralAsync(questionnaire, "name", "email", "telephone");

        // Assert
        mockDataAccessProvider.Verify(dap => dap.PersistNewReferralRequestAsync(It.IsAny<ReferralRequest>()));
    }

    [Test]
    public async Task GenerateReferralAsync_WhenCalled_UpdatesQuestionnaireContactDetails()
    {
        // Arrange
        var questionnaire = new Questionnaire
        {
            IsLsoaProperty = false,
            IncomeBand = IncomeBand.UnderOrEqualTo36000
        };
        mockDataAccessProvider.Setup(dap =>
            dap.PersistNewReferralRequestAsync(It.IsAny<ReferralRequest>())).ReturnsAsync(new ReferralRequest());

        // Act
        var result = await underTest.GenerateReferralAsync(questionnaire, "name", "email", "telephone");

        // Assert
        result.LaContactName.Should().Be("name");
        result.LaContactEmailAddress.Should().Be("email");
        result.LaContactTelephone.Should().Be("telephone");
    }

    [Test]
    public async Task GenerateReferralAsync_WhenCalled_UpdatesQuestionnaireReferralData()
    {
        // Arrange
        var questionnaire = new Questionnaire
        {
            IsLsoaProperty = false,
            IncomeBand = IncomeBand.UnderOrEqualTo36000
        };
        var creationDate = new DateTime(2023, 01, 01, 13, 0, 0);
        var referral = new ReferralRequest
        {
            ReferralCode = "code",
            RequestDate = creationDate
        };
        mockDataAccessProvider.Setup(dap =>
            dap.PersistNewReferralRequestAsync(It.IsAny<ReferralRequest>())).ReturnsAsync(referral);

        // Act
        var result = await underTest.GenerateReferralAsync(questionnaire, "name", "email", "telephone");

        // Assert
        result.ReferralCode.Should().Be("code");
        result.ReferralCreated.Should().Be(creationDate);
    }

    [Test]
    [Ignore("PC-1828: No Live LAs at launch")]
    public async Task
        GenerateReferralAsync_WhenCalledWithEmailAndLocalAuthorityIsLive_SendOneEmailWithReferralCodeWithLiveTemplate()
    {
        // Arrange
        string testCustodianCode = liveCustodianCode;
        const int testReferralId = 12;
        const string testName = "Example Person";
        const string testEmailAddress = "test@example.com";

        var questionnaire = new Questionnaire
        {
            CustodianCode = testCustodianCode,
            IsLsoaProperty = false,
            IncomeBand = IncomeBand.UnderOrEqualTo36000
        };
        var creationDate = new DateTime(2023, 01, 01, 13, 0, 0);

        var referral = new ReferralRequestBuilder(testReferralId);
        referral.WithCustodianCode(testCustodianCode);
        referral.WithRequestDate(creationDate);
        var testReferralRequest = referral.Build();

        mockDataAccessProvider.Setup(dap =>
            dap.PersistNewReferralRequestAsync
            (
                It.Is<ReferralRequest>(rr => rr.CustodianCode == testCustodianCode)
            )).ReturnsAsync(testReferralRequest);
        mockEmailSender.Setup(es =>
            es.SendReferenceCodeEmailForLiveLocalAuthority
            (
                testEmailAddress,
                testName,
                testReferralRequest
            )
        );

        // Act
        var result = await underTest.GenerateReferralAsync
        (
            questionnaire,
            testName,
            testEmailAddress,
            ""
        );

        // Assert
        mockEmailSender.Verify(es => es.SendReferenceCodeEmailForLiveLocalAuthority
        (
            testEmailAddress,
            testName,
            testReferralRequest
        ), Times.Once);
    }

    [Test]
    public async Task
        GenerateReferralAsync_WhenCalledWithEmailAndLocalAuthorityIsPending_SendOneEmailWithReferralCodeWithPendingTemplate()
    {
        // Arrange
        string testCustodianCode = pendingCustodianCode;
        const int testReferralId = 12;
        const string testName = "Example Person";
        const string testEmailAddress = "test@example.com";

        var questionnaire = new Questionnaire
        {
            CustodianCode = testCustodianCode,
            IsLsoaProperty = false,
            IncomeBand = IncomeBand.UnderOrEqualTo36000
        };
        var creationDate = new DateTime(2023, 01, 01, 13, 0, 0);

        var referral = new ReferralRequestBuilder(testReferralId);
        referral.WithCustodianCode(testCustodianCode);
        referral.WithRequestDate(creationDate);
        var testReferralRequest = referral.Build();

        mockDataAccessProvider.Setup(dap =>
            dap.PersistNewReferralRequestAsync
            (
                It.Is<ReferralRequest>(rr => rr.CustodianCode == testCustodianCode)
            )).ReturnsAsync(testReferralRequest);
        mockEmailSender.Setup(es =>
            es.SendReferenceCodeEmailForPendingLocalAuthority(
                testEmailAddress,
                testName,
                testReferralRequest)
        );

        // Act
        var result = await underTest.GenerateReferralAsync
        (
            questionnaire,
            testName,
            testEmailAddress,
            ""
        );

        // Assert
        mockEmailSender.Verify(es => es.SendReferenceCodeEmailForPendingLocalAuthority
        (
            testEmailAddress,
            testName,
            testReferralRequest
        ), Times.Once);
    }

    [Test]
    [Ignore("PC-1828: No Live LAs at launch")]
    public async Task
        GenerateReferralAsync_WhenCalledWithEmailAndLocalAuthorityIsTakingFutureReferrals_SendOneEmailWithReferralCodeWithTakingFutureReferralTemplate()
    {
        // Arrange
        var testCustodianCode = takingFutureReferralsCustodianCode;
        const int testReferralId = 12;
        const string testName = "Example Person";
        const string testEmailAddress = "test@example.com";
    
        var questionnaire = new Questionnaire
        {
            CustodianCode = testCustodianCode,
            IsLsoaProperty = false,
            IncomeBand = IncomeBand.UnderOrEqualTo36000
        };
        var creationDate = new DateTime(2023, 01, 01, 13, 0, 0);
    
        var referral = new ReferralRequestBuilder(testReferralId);
        referral.WithCustodianCode(testCustodianCode);
        referral.WithRequestDate(creationDate);
        var testReferralRequest = referral.Build();
    
        mockDataAccessProvider.Setup(dap =>
            dap.PersistNewReferralRequestAsync
            (
                It.Is<ReferralRequest>(rr => rr.CustodianCode == testCustodianCode)
            )).ReturnsAsync(testReferralRequest);
        mockEmailSender.Setup(es =>
            es.SendReferenceCodeEmailForTakingFutureReferralsLocalAuthority(
                testEmailAddress,
                testName,
                testReferralRequest)
        );
    
        // Act
        var result = await underTest.GenerateReferralAsync
        (
            questionnaire,
            testName,
            testEmailAddress,
            ""
        );
    
        // Assert
        mockEmailSender.Verify(es => es.SendReferenceCodeEmailForTakingFutureReferralsLocalAuthority
        (
            testEmailAddress,
            testName,
            testReferralRequest
        ), Times.Once);
    }

    [Test]
    public async Task GenerateReferralAsync_WhenCalledWithoutEmail_DoesNotSendEmail()
    {
        // Arrange
        const string testReferralCode = "referral code";
        const string testName = "Example Person";
        var questionnaire = new Questionnaire
        {
            IsLsoaProperty = false,
            IncomeBand = IncomeBand.UnderOrEqualTo36000
        };
        var creationDate = new DateTime(2023, 01, 01, 13, 0, 0);
        var referral = new ReferralRequest
        {
            ReferralCode = testReferralCode,
            RequestDate = creationDate
        };
        mockDataAccessProvider.Setup(dap =>
            dap.PersistNewReferralRequestAsync(It.IsAny<ReferralRequest>())).ReturnsAsync(referral);
        mockEmailSender.Setup(es =>
            es.SendReferenceCodeEmailForLiveLocalAuthority
            (
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<ReferralRequest>()
            )
        );

        // Act
        var result = await underTest.GenerateReferralAsync
        (
            questionnaire,
            testName,
            "",
            ""
        );

        // Assert
        mockEmailSender.Verify(es => es.SendReferenceCodeEmailForLiveLocalAuthority
        (
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<ReferralRequest>()
        ), Times.Never);
    }

    [Test]
    public async Task RecordNotificationConsentAsync_WhenCalledWithLaContactEmail_PersistsConsent()
    {
        // Arrange
        var questionnaire = new Questionnaire
        {
            LaContactEmailAddress = "test@example.com",
            ReferralCode = "referral code"
        };

        // Act
        await underTest.RecordNotificationConsentAsync(questionnaire, true);

        // Assert
        mockDataAccessProvider.Verify(dap =>
            dap.PersistNotificationConsentAsync("referral code", It.IsAny<NotificationDetails>()));
    }

    [Test]
    public async Task RecordNotificationConsentAsync_WhenCalledWithLaContactEmailAndConsent_UsesLaContactEmail()
    {
        // Arrange
        var questionnaire = new Questionnaire
        {
            LaContactEmailAddress = "test@example.com",
            ReferralCode = "referral code"
        };

        // Act
        var result = await underTest.RecordNotificationConsentAsync(questionnaire, true);

        // Assert
        result.NotificationConsent.Should().BeTrue();
        result.NotificationEmailAddress.Should().Be("test@example.com");
    }

    [Test]
    public async Task RecordNotificationConsentAsync_WhenCalledWithLaContactEmailAndNoConsent_UsesNullEmail()
    {
        // Arrange
        var questionnaire = new Questionnaire
        {
            LaContactEmailAddress = "test@example.com",
            ReferralCode = "referral code"
        };

        // Act
        var result = await underTest.RecordNotificationConsentAsync(questionnaire, false);

        // Assert
        result.NotificationConsent.Should().BeFalse();
        result.NotificationEmailAddress.Should().BeNull();
    }

    [TestCase(true, "test@example.com")]
    [TestCase(false, "")]
    [Ignore("PC-1828: No Live LAs at launch")]
    public async Task
        RecordConfirmationAndNotificationConsentAsync_WhenConfirmationConsentGrantedAndEmailGivenAndLocalAuthorityIsLive_SendsOneLiveTemplateEmailWithReferralCode
        (
            bool notificationConsentGranted,
            string notificationEmailAddress
        )
    {
        // Arrange
        string testCustodianCode = liveCustodianCode;
        const string testReferralCode = "referral code";
        const int testReferralId = 12;
        const string testName = "Example Person";
        const string testEmailAddress = "test@example.com";
        var questionnaire = new Questionnaire
        {
            LaContactName = testName,
            LaContactEmailAddress = testEmailAddress,
            ReferralCode = testReferralCode,
            CustodianCode = testCustodianCode,
            IsLsoaProperty = false,
            IncomeBand = IncomeBand.UnderOrEqualTo36000
        };
        var creationDate = new DateTime(2023, 01, 01, 13, 0, 0);

        var referral = new ReferralRequestBuilder(testReferralId);
        referral.WithCustodianCode(testCustodianCode);
        referral.WithRequestDate(creationDate);
        var testReferralRequest = referral.Build();
        mockEmailSender.Setup(es =>
            es.SendReferenceCodeEmailForLiveLocalAuthority(
                testEmailAddress,
                testName,
                testReferralRequest)
        );

        // Act
        var result = await underTest.RecordConfirmationAndNotificationConsentAsync
        (
            questionnaire,
            notificationConsentGranted,
            notificationEmailAddress,
            true,
            testEmailAddress
        );

        // Assert
        mockEmailSender.Verify(es => es.SendReferenceCodeEmailForLiveLocalAuthority(
                testEmailAddress,
                testName,
                It.IsAny<ReferralRequest>()),
            Times.Once);
    }

    [TestCase(true, "test@example.com")]
    [TestCase(false, "")]
    public async Task
        RecordConfirmationAndNotificationConsentAsync_WhenConfirmationConsentGrantedAndEmailGivenAndLocalAuthorityIsPending_SendsOnePendingTemplateEmailWithReferralCode
        (
            bool notificationConsentGranted,
            string notificationEmailAddress
        )
    {
        // Arrange
        string testCustodianCode = pendingCustodianCode;
        const string testReferralCode = "referral code";
        const int testReferralId = 12;
        const string testName = "Example Person";
        const string testEmailAddress = "test@example.com";
        var questionnaire = new Questionnaire
        {
            LaContactName = testName,
            LaContactEmailAddress = testEmailAddress,
            ReferralCode = testReferralCode,
            CustodianCode = testCustodianCode,
            IsLsoaProperty = false,
            IncomeBand = IncomeBand.UnderOrEqualTo36000
        };
        var creationDate = new DateTime(2023, 01, 01, 13, 0, 0);

        var referral = new ReferralRequestBuilder(testReferralId);
        referral.WithCustodianCode(testCustodianCode);
        referral.WithRequestDate(creationDate);
        var testReferralRequest = referral.Build();

        mockEmailSender.Setup(es =>
            es.SendReferenceCodeEmailForPendingLocalAuthority(
                testEmailAddress,
                testName,
                testReferralRequest)
        );

        // Act
        var result = await underTest.RecordConfirmationAndNotificationConsentAsync
        (
            questionnaire,
            notificationConsentGranted,
            notificationEmailAddress,
            true,
            testEmailAddress
        );

        // Assert
        mockEmailSender.Verify(es => es.SendReferenceCodeEmailForPendingLocalAuthority(
                testEmailAddress,
                testName,
                It.IsAny<ReferralRequest>()),
            Times.Once);
    }

    [TestCase(true, "test@example.com")]
    [TestCase(false, "")]
    public async Task
        RecordConfirmationAndNotificationConsentAsync_WhenConfirmationConsentNotGrantedAndEmailNotGiven_DoesNotSendEmail
        (
            bool notificationConsentGranted,
            string notificationEmailAddress
        )
    {
        // Arrange
        const string testCustodianCode = "1234";
        const string testReferralCode = "referral code";
        const string testName = "Example Person";
        var questionnaire = new Questionnaire
        {
            LaContactName = testName,
            ReferralCode = testReferralCode,
            CustodianCode = testCustodianCode,
        };
        mockEmailSender.Setup(es =>
            es.SendReferenceCodeEmailForLiveLocalAuthority
            (
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<ReferralRequest>()
            )
        );

        // Act
        var result = await underTest.RecordConfirmationAndNotificationConsentAsync
        (
            questionnaire,
            notificationConsentGranted,
            notificationEmailAddress,
            false,
            ""
        );

        // Assert
        mockEmailSender.Verify(es => es.SendReferenceCodeEmailForLiveLocalAuthority
        (
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<ReferralRequest>()
        ), Times.Never);
    }


    [Test]
    public async Task UpdateQuestionnaire_EditStarts_CreatesUneditedData()
    {
        // Arrange
        var questionnaire = new Questionnaire();

        mockQuestionFlowService.Setup(qfs => qfs.NextStep(
            It.IsAny<QuestionFlowStep>(),
            It.IsAny<Questionnaire>(),
            It.IsAny<QuestionFlowStep>()
        )).Returns(QuestionFlowStep.SelectLocalAuthority);

        // Act
        var result = await underTest.UpdateAddressAsync(questionnaire, new Address(), QuestionFlowStep.Address);

        // Assert
        result.UneditedData.Should().NotBeNull();
    }

    [Test]
    public async Task UpdateQuestionnaire_NotEditing_NoUneditedData()
    {
        // Arrange
        var questionnaire = new Questionnaire();

        mockQuestionFlowService.Setup(qfs => qfs.NextStep(
            It.IsAny<QuestionFlowStep>(),
            It.IsAny<Questionnaire>(),
            It.IsAny<QuestionFlowStep>()
        )).Returns(QuestionFlowStep.SelectLocalAuthority);

        // Act
        var result = await underTest.UpdateAddressAsync(questionnaire, new Address(), null);

        // Assert
        result.UneditedData.Should().BeNull();
    }

    [Test]
    public async Task UpdateQuestionnaire_EditWhileExistingData_UneditedDataPreserved()
    {
        // Arrange
        var questionnaire = new Questionnaire() { UneditedData = new Questionnaire() };

        mockQuestionFlowService.Setup(qfs => qfs.NextStep(
            It.IsAny<QuestionFlowStep>(),
            It.IsAny<Questionnaire>(),
            It.IsAny<QuestionFlowStep>()
        )).Returns(QuestionFlowStep.SelectLocalAuthority);

        // Act
        var result = await underTest.UpdateAddressAsync(questionnaire, new Address(), QuestionFlowStep.Address);

        // Assert
        result.UneditedData.Should().Be(new Questionnaire());
    }

    [Test]
    public async Task UpdateQuestionnaire_EditComplete_DataCommitted()
    {
        // Arrange
        var questionnaire = new Questionnaire() { UneditedData = new Questionnaire() };

        mockQuestionFlowService.Setup(qfs => qfs.NextStep(
            It.IsAny<QuestionFlowStep>(),
            It.IsAny<Questionnaire>(),
            It.IsAny<QuestionFlowStep>()
        )).Returns(QuestionFlowStep.CheckAnswers);

        // Act
        var result = await underTest.UpdateAddressAsync(questionnaire, new Address(), QuestionFlowStep.Address);

        // Assert
        result.UneditedData.Should().BeNull();
    }
}