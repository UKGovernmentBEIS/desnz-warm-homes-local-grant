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
            EpcDetailsAreCorrect = EpcConfirmation.Yes,
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
    public void UneditedData_WhenCreated_CopiesAllAnswers()
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
    public void UneditedData_WhenCommitEdits_UneditedDataIsNull()
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
            EpcDetailsAreCorrect = null,
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
            EpcDetailsAreCorrect = EpcConfirmation.No,
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
    public void EffectiveEpcBand_ForUnknownEpcDetails_IgnoresEpcDetails()
    {
        // Arrange
        var questionnaire = new Questionnaire()
        {
            EpcDetailsAreCorrect = EpcConfirmation.Unknown,
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
            EpcDetailsAreCorrect = EpcConfirmation.Yes,
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
            EpcDetailsAreCorrect = EpcConfirmation.Yes,
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
            EpcDetailsAreCorrect = EpcConfirmation.Yes,
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
    
    [Test]
    public void FoundEpcBandIsTooHigh_ForExpiredEpc_ReturnsFalse()
    {
        // Arrange
        var questionnaire = new Questionnaire()
        {
            EpcDetails = new EpcDetails
            {
                EpcRating = EpcRating.A,
                ExpiryDate = DateTime.MinValue
            }
        };
        
        // Act
        var result = questionnaire.FoundEpcBandIsTooHigh;
        
        // Assert
        result.Should().Be(false);
    }

    [TestCase(HasGasBoiler.No, Country.England, OwnershipStatus.OwnerOccupancy, EpcConfirmation.Yes, EpcRating.D, false, IncomeBand.UnderOrEqualTo31000, true)] // Eligible low income
    [TestCase(HasGasBoiler.No, Country.England, OwnershipStatus.OwnerOccupancy, null, null, false, IncomeBand.UnderOrEqualTo31000, true)] // Eligible no EPC found
    [TestCase(HasGasBoiler.No, Country.England, OwnershipStatus.OwnerOccupancy, EpcConfirmation.No, EpcRating.A, false, IncomeBand.UnderOrEqualTo31000, true)] // Eligible wrong EPC found
    [TestCase(HasGasBoiler.No, Country.England, OwnershipStatus.OwnerOccupancy, EpcConfirmation.Unknown, EpcRating.A, false, IncomeBand.UnderOrEqualTo31000, true)] // Eligible unsure EPC found
    [TestCase(HasGasBoiler.No, Country.England, OwnershipStatus.OwnerOccupancy, EpcConfirmation.Yes, EpcRating.D, true, IncomeBand.GreaterThan31000, true)] // Eligible high income but LSOA
    [TestCase(HasGasBoiler.Yes, Country.England, OwnershipStatus.OwnerOccupancy, EpcConfirmation.Yes, EpcRating.D, false, IncomeBand.UnderOrEqualTo31000, false)] // Ineligible gas boiler
    [TestCase(HasGasBoiler.No, Country.Scotland, OwnershipStatus.OwnerOccupancy, EpcConfirmation.Yes, EpcRating.D, false, IncomeBand.UnderOrEqualTo31000, false)] // Ineligible country
    [TestCase(HasGasBoiler.No, Country.Wales, OwnershipStatus.OwnerOccupancy, EpcConfirmation.Yes, EpcRating.D, false, IncomeBand.UnderOrEqualTo31000, false)] // Ineligible country
    [TestCase(HasGasBoiler.No, Country.NorthernIreland, OwnershipStatus.OwnerOccupancy, EpcConfirmation.Yes, EpcRating.D, false, IncomeBand.UnderOrEqualTo31000, false)] // Ineligible country
    [TestCase(HasGasBoiler.No, Country.England, OwnershipStatus.Landlord, EpcConfirmation.Yes, EpcRating.D, false, IncomeBand.UnderOrEqualTo31000, false)] // Ineligible ownership
    [TestCase(HasGasBoiler.No, Country.England, OwnershipStatus.PrivateTenancy, EpcConfirmation.Yes, EpcRating.D, false, IncomeBand.UnderOrEqualTo31000, false)] // Ineligible ownership
    [TestCase(HasGasBoiler.No, Country.England, OwnershipStatus.OwnerOccupancy, EpcConfirmation.Yes, EpcRating.A, false, IncomeBand.UnderOrEqualTo31000, false)] // Ineligible EPC rating
    [TestCase(HasGasBoiler.No, Country.England, OwnershipStatus.OwnerOccupancy, EpcConfirmation.Yes, EpcRating.B, false, IncomeBand.UnderOrEqualTo31000, false)] // Ineligible EPC rating
    [TestCase(HasGasBoiler.No, Country.England, OwnershipStatus.OwnerOccupancy, EpcConfirmation.Yes, EpcRating.C, false, IncomeBand.UnderOrEqualTo31000, false)] // Ineligible EPC rating
    [TestCase(HasGasBoiler.No, Country.England, OwnershipStatus.OwnerOccupancy, EpcConfirmation.Yes, EpcRating.D, false, IncomeBand.GreaterThan31000, false)] // Ineligible high income
    public void IsEligibleForHug2_ForVariousAnswers_IsCorrect(
        HasGasBoiler hasGasBoiler,
        Country country,
        OwnershipStatus ownershipStatus,
        EpcConfirmation? epcDetailsAreCorrect,
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
    
    [Test]
    public void IncomeBandIsValid_ForNullCustodianCode_ReturnsFalse()
    {
        // Arrange
        var questionnaire = new Questionnaire()
        {
            CustodianCode = null,
            IncomeBand = IncomeBand.GreaterThan31000
        };
        
        // Act
        var result = questionnaire.IncomeBandIsValid;
        
        // Assert
        result.Should().BeFalse();
    }
    
    [TestCase(IncomeBand.GreaterThan31000)]
    [TestCase(IncomeBand.GreaterThan34500)]
    public void IncomeIsTooHigh_ForHighIncomeBandAndNonLsoa_ReturnsTrue(IncomeBand incomeBand)
    {
        // Arrange
        var questionnaire = new Questionnaire()
        {
            IncomeBand = incomeBand,
            IsLsoaProperty = false
        };
        
        // Act
        var result = questionnaire.IncomeIsTooHigh;
        
        // Assert
        result.Should().BeTrue();
    }
    
    [TestCase(IncomeBand.GreaterThan31000)]
    [TestCase(IncomeBand.GreaterThan34500)]
    public void IncomeIsTooHigh_ForHighIncomeBandAndLsoa_ReturnsFalse(IncomeBand incomeBand)
    {
        // Arrange
        var questionnaire = new Questionnaire()
        {
            IncomeBand = incomeBand,
            IsLsoaProperty = true
        };
        
        // Act
        var result = questionnaire.IncomeIsTooHigh;
        
        // Assert
        result.Should().BeFalse();
    }
    
    [TestCase(IncomeBand.UnderOrEqualTo31000, true)]
    [TestCase(IncomeBand.UnderOrEqualTo31000, false)]
    [TestCase(IncomeBand.UnderOrEqualTo34500, true)]
    [TestCase(IncomeBand.UnderOrEqualTo34500, false)]
    public void IncomeIsTooHigh_ForLowIncomeBand_ReturnsFalse(IncomeBand incomeBand, bool isLsoa)
    {
        // Arrange
        var questionnaire = new Questionnaire()
        {
            IncomeBand = incomeBand,
            IsLsoaProperty = isLsoa
        };
        
        // Act
        var result = questionnaire.IncomeIsTooHigh;
        
        // Assert
        result.Should().BeFalse();
    }
    
    [Test]
    public void IncomeBandIsValid_ForNullIncomeBand_ReturnsFalse()
    {
        // Arrange
        var questionnaire = new Questionnaire()
        {
            CustodianCode = "9052", // Aberdeenshire is configure with a £31,000 threshold and shouldn't change as it isn't taking part in HUG2
            IncomeBand = null
        };
        
        // Act
        var result = questionnaire.IncomeBandIsValid;
        
        // Assert
        result.Should().BeFalse();
    }
    
    [TestCase(IncomeBand.GreaterThan34500)]
    [TestCase(IncomeBand.UnderOrEqualTo34500)]
    public void IncomeBandIsValid_ForBadIncomeBand_ReturnsFalse(IncomeBand incomeBand)
    {
        // Arrange
        var questionnaire = new Questionnaire()
        {
            CustodianCode = "9052", // Aberdeenshire is configure with a £31,000 threshold and shouldn't change as it isn't taking part in HUG2
            IncomeBand = incomeBand
        };
        
        // Act
        var result = questionnaire.IncomeBandIsValid;
        
        // Assert
        result.Should().BeFalse();
    }
    
    [TestCase(IncomeBand.GreaterThan31000)]
    [TestCase(IncomeBand.UnderOrEqualTo31000)]
    public void IncomeBandIsValid_ForGoodIncomeBand_ReturnsTrue(IncomeBand incomeBand)
    {
        // Arrange
        var questionnaire = new Questionnaire()
        {
            CustodianCode = "9052", // Aberdeenshire is configure with a £31,000 threshold and shouldn't change as it isn't taking part in HUG2
            IncomeBand = incomeBand
        };
        
        // Act
        var result = questionnaire.IncomeBandIsValid;
        
        // Assert
        result.Should().BeTrue();
    }
}
