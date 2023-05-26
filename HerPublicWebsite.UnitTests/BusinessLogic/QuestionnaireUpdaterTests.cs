using System;
using System.Threading.Tasks;
using FluentAssertions;
using HerPublicWebsite.BusinessLogic;
using HerPublicWebsite.BusinessLogic.ExternalServices.EpbEpc;
using HerPublicWebsite.BusinessLogic.Models;
using HerPublicWebsite.BusinessLogic.Models.Enums;
using HerPublicWebsite.BusinessLogic.Services.EligiblePostcode;
using Moq;
using NUnit.Framework;

namespace Tests.BusinessLogic;

[TestFixture]
public class QuestionnaireUpdaterTests
{
    private QuestionnaireUpdater underTest;
    private Mock<IEpcApi> mockEpcApi;
    private Mock<IEligiblePostcodeService> mockPostCodeService;
    private Mock<IDataAccessProvider> mockDataAccessProvider;
    
    
    [SetUp]
    public void Setup()
    {
        mockEpcApi = new Mock<IEpcApi>();
        mockPostCodeService = new Mock<IEligiblePostcodeService>();
        mockDataAccessProvider = new Mock<IDataAccessProvider>();
        underTest = new QuestionnaireUpdater(mockEpcApi.Object, mockPostCodeService.Object, mockDataAccessProvider.Object);
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
        var result = await underTest.UpdateAddressAsync(questionnaire, address);
        
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
        var result = await underTest.UpdateAddressAsync(questionnaire, address);
        
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
        var result = await underTest.UpdateAddressAsync(questionnaire, address);
        
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
        var result = underTest.UpdateLocalAuthority(questionnaire, "new code");
        
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
            IncomeBand = IncomeBand.UnderOrEqualTo31000,
            HasGasBoiler = HasGasBoiler.No
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
            IncomeBand = IncomeBand.UnderOrEqualTo31000,
            HasGasBoiler = HasGasBoiler.No
        };
        mockDataAccessProvider.Setup(dap =>
            dap.PersistNewReferralRequestAsync(It.IsAny<ReferralRequest>())).ReturnsAsync(new ReferralRequest());
        
        // Act
        var result = await underTest.GenerateReferralAsync(questionnaire, "name", "email", "telephone");
        
        // Assert
        result.ContactDetails.FullName.Should().Be("name");
        result.ContactDetails.LaContactEmailAddress.Should().Be("email");
        result.ContactDetails.LaContactTelephone.Should().Be("telephone");
    }
    
    [Test]
    public async Task GenerateReferralAsync_WhenCalled_UpdatesQuestionnaireReferralData()
    {
        // Arrange
        var questionnaire = new Questionnaire
        {
            IsLsoaProperty = false,
            IncomeBand = IncomeBand.UnderOrEqualTo31000,
            HasGasBoiler = HasGasBoiler.No
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
        result.Hug2ReferralId.Should().Be("code");
        result.ReferralCreated.Should().Be(creationDate);
    }
}
