using System;
using FluentAssertions;
using HerPublicWebsite.BusinessLogic.Models;
using HerPublicWebsite.BusinessLogic.Models.Enums;
using NUnit.Framework;

namespace Tests.BusinessLogic.Models;

[TestFixture]
public class QuestionnaireTests
{
    private Questionnaire InitializeQuestionnaire()
    {
        return new Questionnaire
        {
            Country = Country.England,
            OwnershipStatus = OwnershipStatus.OwnerOccupancy,
            AddressLine1 = "Address Line 1",
            AddressLine2 = "Address Line 2",
            AddressTown = "A Town",
            AddressCounty = "A County",
            AddressPostcode = "PST C0D",
            CustodianCode = "5210",
            LocalAuthorityConfirmed = true,
            Uprn = "123456789123",
            EpcDetails = new EpcDetails(),
            EpcDetailsAreCorrect = true,
            IsLsoaProperty = true,
            HasGasBoiler = HasGasBoiler.No,
            IncomeBand = IncomeBand.UnderOrEqualTo31000,
            ReferralCreated = default,
            ReferralCode = "HUG21023",
            LaContactName = "Contact Name",
            LaCanContactByEmail = true,
            LaCanContactByPhone = true,
            LaContactEmailAddress = "person@place.com",
            LaContactTelephone = "07123456789",
            NotificationConsent = true,
            ConfirmationConsent = true,
            NotificationEmailAddress = "person@place.com",
            ConfirmationEmailAddress = "person@place.com",
            UneditedData = new Questionnaire()
        };
    }
    
    [Test]
    public void CopiesAllAnswers()
    {
        // Arrange
        var questionnaire = InitializeQuestionnaire();

        // Act
        questionnaire.CreateUneditedData();

        // Assert
        foreach (var propertyInfo in questionnaire.GetType().GetProperties())
        {
            if (
                propertyInfo.Name.Equals(nameof(questionnaire.UneditedData))
            )
            {
                continue;
            }
            
            propertyInfo.GetValue(questionnaire.UneditedData).Should().NotBeNull();
        }
    }
    
    [Test]
    public void CommitEditsResetsUneditedData()
    {
        // Arrange
        var questionnaire = InitializeQuestionnaire();
        
        // Act
        questionnaire.CommitEdits();
        
        // Assert
        questionnaire.UneditedData.Should().BeNull();
    }
    
    [Test]
    public void EffectiveEpcBand_WithNoEpcDetails_ReturnsUnknown()
    {
        // Arrange
        var questionnaire = new Questionnaire()
        {
            EpcDetailsAreCorrect = false,
            EpcDetails = null
        };
        
        // Act
        var result = questionnaire.EffectiveEpcBand;
        
        // Assert
        result.Should().Be(EpcRating.Unknown);
    }
    
    [Test]
    public void EffectiveEpcBand_ForIncorrectEpcDetails_IgnoresEpcDetails()
    {
        // Arrange
        var questionnaire = new Questionnaire()
        {
            EpcDetailsAreCorrect = false,
            EpcDetails = new EpcDetails
            {
                EpcRating = EpcRating.A
            }
        };
        
        // Act
        var result = questionnaire.EffectiveEpcBand;
        
        // Assert
        result.Should().Be(EpcRating.Unknown);
    }
    
    [Test]
    public void EffectiveEpcBand_ForCorrectEpcDetails_UsesEpcDetailsBand()
    {
        // Arrange
        var questionnaire = new Questionnaire()
        {
            EpcDetailsAreCorrect = true,
            EpcDetails = new EpcDetails
            {
                EpcRating = EpcRating.A
            }
        };
        
        // Act
        var result = questionnaire.EffectiveEpcBand;
        
        // Assert
        result.Should().Be(EpcRating.A);
    }
    
    [Test]
    public void EffectiveEpcBand_ForExpiredEpcDetails_ReturnsExpired()
    {
        // Arrange
        var questionnaire = new Questionnaire()
        {
            EpcDetailsAreCorrect = true,
            EpcDetails = new EpcDetails
            {
                EpcRating = EpcRating.A,
                ExpiryDate = new DateTime(2010, 01, 01)
            }
        };
        
        // Act
        var result = questionnaire.EffectiveEpcBand;
        
        // Assert
        result.Should().Be(EpcRating.Expired);
    }
    
    [Test]
    public void EffectiveEpcBand_ForEpcDetailsWithNullRating_ReturnsUnknown()
    {
        // Arrange
        var questionnaire = new Questionnaire()
        {
            EpcDetailsAreCorrect = true,
            EpcDetails = new EpcDetails
            {
                EpcRating = null
            }
        };
        
        // Act
        var result = questionnaire.EffectiveEpcBand;
        
        // Assert
        result.Should().Be(EpcRating.Unknown);
    }
    
    [TestCase(EpcRating.A, true)]
    [TestCase(EpcRating.B, true)]
    [TestCase(EpcRating.C, true)]
    [TestCase(EpcRating.D, false)]
    [TestCase(EpcRating.E, false)]
    [TestCase(EpcRating.F, false)]
    [TestCase(EpcRating.G, false)]
    public void FoundEpcBandIsTooHigh_ForEpcDetails_ReturnsTrueForABorC(EpcRating rating, bool expectedTooHighValue)
    {
        // Arrange
        var questionnaire = new Questionnaire()
        {
            EpcDetails = new EpcDetails
            {
                EpcRating = rating
            }
        };
        
        // Act
        var result = questionnaire.FoundEpcBandIsTooHigh;
        
        // Assert
        result.Should().Be(expectedTooHighValue);
    }
    
    [Test]
    public void FoundEpcBandIsTooHigh_ForMissingEpcDetails_ReturnsFalse()
    {
        // Arrange
        var questionnaire = new Questionnaire()
        {
            EpcDetails = null
        };
        
        // Act
        var result = questionnaire.FoundEpcBandIsTooHigh;
        
        // Assert
        result.Should().Be(false);
    }
    
    [Test]
    public void FoundEpcBandIsTooHigh_ForMissingEpcBand_ReturnsFalse()
    {
        // Arrange
        var questionnaire = new Questionnaire()
        {
            EpcDetails = new EpcDetails
            {
                EpcRating = null
            }
        };
        
        // Act
        var result = questionnaire.FoundEpcBandIsTooHigh;
        
        // Assert
        result.Should().Be(false);
    }

    [TestCase(HasGasBoiler.No, Country.England, OwnershipStatus.OwnerOccupancy, true, EpcRating.D, false, IncomeBand.UnderOrEqualTo31000, true)] // Eligible low income
    [TestCase(HasGasBoiler.No, Country.England, OwnershipStatus.OwnerOccupancy, null, null, false, IncomeBand.UnderOrEqualTo31000, true)] // Eligible no EPC found
    [TestCase(HasGasBoiler.No, Country.England, OwnershipStatus.OwnerOccupancy, false, EpcRating.A, false, IncomeBand.UnderOrEqualTo31000, true)] // Eligible wrong EPC found
    [TestCase(HasGasBoiler.No, Country.England, OwnershipStatus.OwnerOccupancy, true, EpcRating.D, true, IncomeBand.GreaterThan31000, true)] // Eligible high income but LSOA
    [TestCase(HasGasBoiler.Yes, Country.England, OwnershipStatus.OwnerOccupancy, true, EpcRating.D, false, IncomeBand.UnderOrEqualTo31000, false)] // Ineligible gas boiler
    [TestCase(HasGasBoiler.No, Country.Scotland, OwnershipStatus.OwnerOccupancy, true, EpcRating.D, false, IncomeBand.UnderOrEqualTo31000, false)] // Ineligible country
    [TestCase(HasGasBoiler.No, Country.Wales, OwnershipStatus.OwnerOccupancy, true, EpcRating.D, false, IncomeBand.UnderOrEqualTo31000, false)] // Ineligible country
    [TestCase(HasGasBoiler.No, Country.NorthernIreland, OwnershipStatus.OwnerOccupancy, true, EpcRating.D, false, IncomeBand.UnderOrEqualTo31000, false)] // Ineligible country
    [TestCase(HasGasBoiler.No, Country.England, OwnershipStatus.Landlord, true, EpcRating.D, false, IncomeBand.UnderOrEqualTo31000, false)] // Ineligible ownership
    [TestCase(HasGasBoiler.No, Country.England, OwnershipStatus.PrivateTenancy, true, EpcRating.D, false, IncomeBand.UnderOrEqualTo31000, false)] // Ineligible ownership
    [TestCase(HasGasBoiler.No, Country.England, OwnershipStatus.OwnerOccupancy, true, EpcRating.A, false, IncomeBand.UnderOrEqualTo31000, false)] // Ineligible EPC rating
    [TestCase(HasGasBoiler.No, Country.England, OwnershipStatus.OwnerOccupancy, true, EpcRating.B, false, IncomeBand.UnderOrEqualTo31000, false)] // Ineligible EPC rating
    [TestCase(HasGasBoiler.No, Country.England, OwnershipStatus.OwnerOccupancy, true, EpcRating.C, false, IncomeBand.UnderOrEqualTo31000, false)] // Ineligible EPC rating
    [TestCase(HasGasBoiler.No, Country.England, OwnershipStatus.OwnerOccupancy, true, EpcRating.D, false, IncomeBand.GreaterThan31000, false)] // Ineligible high income
    public void IsEligibleForHug2_ForVariousAnswers_IsCorrect(
        HasGasBoiler hasGasBoiler,
        Country country,
        OwnershipStatus ownershipStatus,
        bool? epcDetailsAreCorrect,
        EpcRating? epcRating,
        bool isLsoaPostCode,
        IncomeBand incomeBand,
        bool isEligible)
    {
        // Arrange
        var underTest = new Questionnaire
        {
            HasGasBoiler = hasGasBoiler,
            Country = country,
            OwnershipStatus = ownershipStatus,
            EpcDetailsAreCorrect = epcDetailsAreCorrect,
            IsLsoaProperty = isLsoaPostCode,
            IncomeBand = incomeBand,
        };
        if (epcRating is not null)
        {
            underTest.EpcDetails = new EpcDetails
            {
                EpcRating = epcRating
            };
        }
        
        // Act
        var result = underTest.IsEligibleForHug2;
        
        // Assert
        result.Should().Be(isEligible);
    }

    [Test]
    public void LocalAuthorityName_ForBadCustodianCode_ReturnsUnknown()
    {
        // Arrange
        var underTest = new Questionnaire
        {
            CustodianCode = "bad_code"
        };
        
        // Act
        var result = underTest.LocalAuthorityName;
        
        // Assert
        result.Should().Be("unrecognised local authority");
    }
    
    
    [Test]
    public void LocalAuthorityName_ForGoodCustodianCode_ReturnsLaName()
    {
        // Arrange
        var underTest = new Questionnaire
        {
            CustodianCode = "9052"
        };
        
        // Act
        var result = underTest.LocalAuthorityName;
        
        // Assert
        result.Should().Be("Aberdeenshire Council");
    }
    
    [Test]
    public void LocalAuthorityWebsite_ForBadCustodianCode_ReturnsUnknown()
    {
        // Arrange
        var underTest = new Questionnaire
        {
            CustodianCode = "bad_code"
        };
        
        // Act
        var result = underTest.LocalAuthorityWebsite;
        
        // Assert
        result.Should().Be("unrecognised local authority");
    }
    
    
    [Test]
    public void LocalAuthorityWebsite_ForGoodCustodianCode_ReturnsLaName()
    {
        // Arrange
        var underTest = new Questionnaire
        {
            CustodianCode = "9052"
        };
        
        // Act
        var result = underTest.LocalAuthorityWebsite;
        
        // Assert
        result.Should().Be("https://www.aberdeenshire.gov.uk/");
    }
}
