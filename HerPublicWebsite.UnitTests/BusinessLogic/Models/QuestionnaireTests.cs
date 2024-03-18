using System;
using FluentAssertions;
using HerPublicWebsite.BusinessLogic.Models;
using HerPublicWebsite.BusinessLogic.Models.Enums;
using NUnit.Framework;
using Tests.Helpers;

namespace Tests.BusinessLogic.Models;

[TestFixture]
public class QuestionnaireTests
{
    [Test]
    public void UneditedData_WhenCreated_CopiesAllAnswers()
    {
        // Arrange
        var questionnaire = QuestionnaireHelper.InitializeQuestionnaire();

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
        var questionnaire = QuestionnaireHelper.InitializeQuestionnaire();
        
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

    [TestCase(HasGasBoiler.No, Country.England, OwnershipStatus.OwnerOccupancy, EpcConfirmation.Yes, EpcRating.D, false, IncomeBand.UnderOrEqualTo36000, true)] // Eligible low income
    [TestCase(HasGasBoiler.No, Country.England, OwnershipStatus.OwnerOccupancy, null, null, false, IncomeBand.UnderOrEqualTo36000, true)] // Eligible no EPC found
    [TestCase(HasGasBoiler.No, Country.England, OwnershipStatus.OwnerOccupancy, EpcConfirmation.No, EpcRating.A, false, IncomeBand.UnderOrEqualTo36000, true)] // Eligible wrong EPC found
    [TestCase(HasGasBoiler.No, Country.England, OwnershipStatus.OwnerOccupancy, EpcConfirmation.Unknown, EpcRating.A, false, IncomeBand.UnderOrEqualTo36000, true)] // Eligible unsure EPC found
    [TestCase(HasGasBoiler.No, Country.England, OwnershipStatus.OwnerOccupancy, EpcConfirmation.Yes, EpcRating.D, true, IncomeBand.GreaterThan36000, true)] // Eligible high income but LSOA
    [TestCase(HasGasBoiler.Yes, Country.England, OwnershipStatus.OwnerOccupancy, EpcConfirmation.Yes, EpcRating.D, false, IncomeBand.UnderOrEqualTo36000, false)] // Ineligible gas boiler
    [TestCase(HasGasBoiler.No, Country.Scotland, OwnershipStatus.OwnerOccupancy, EpcConfirmation.Yes, EpcRating.D, false, IncomeBand.UnderOrEqualTo36000, false)] // Ineligible country
    [TestCase(HasGasBoiler.No, Country.Wales, OwnershipStatus.OwnerOccupancy, EpcConfirmation.Yes, EpcRating.D, false, IncomeBand.UnderOrEqualTo36000, false)] // Ineligible country
    [TestCase(HasGasBoiler.No, Country.NorthernIreland, OwnershipStatus.OwnerOccupancy, EpcConfirmation.Yes, EpcRating.D, false, IncomeBand.UnderOrEqualTo36000, false)] // Ineligible country
    [TestCase(HasGasBoiler.No, Country.England, OwnershipStatus.Landlord, EpcConfirmation.Yes, EpcRating.D, false, IncomeBand.UnderOrEqualTo36000, false)] // Ineligible ownership
    [TestCase(HasGasBoiler.No, Country.England, OwnershipStatus.PrivateTenancy, EpcConfirmation.Yes, EpcRating.D, false, IncomeBand.UnderOrEqualTo36000, false)] // Ineligible ownership
    [TestCase(HasGasBoiler.No, Country.England, OwnershipStatus.OwnerOccupancy, EpcConfirmation.Yes, EpcRating.A, false, IncomeBand.UnderOrEqualTo36000, false)] // Ineligible EPC rating
    [TestCase(HasGasBoiler.No, Country.England, OwnershipStatus.OwnerOccupancy, EpcConfirmation.Yes, EpcRating.B, false, IncomeBand.UnderOrEqualTo36000, false)] // Ineligible EPC rating
    [TestCase(HasGasBoiler.No, Country.England, OwnershipStatus.OwnerOccupancy, EpcConfirmation.Yes, EpcRating.C, false, IncomeBand.UnderOrEqualTo36000, false)] // Ineligible EPC rating
    [TestCase(HasGasBoiler.No, Country.England, OwnershipStatus.OwnerOccupancy, EpcConfirmation.Yes, EpcRating.D, false, IncomeBand.GreaterThan36000, false)] // Ineligible high income
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
            IncomeBand = IncomeBand.GreaterThan36000
        };
        
        // Act
        var result = questionnaire.IncomeBandIsValid;
        
        // Assert
        result.Should().BeFalse();
    }
    
    #pragma warning disable CS0618 
    [TestCase(IncomeBand.GreaterThan31000)] //Test case tests backwards compatibility with obsolete bands
    [TestCase(IncomeBand.GreaterThan34500)] //Test case tests backwards compatibility with obsolete bands
    [TestCase(IncomeBand.GreaterThan36000)]
    #pragma warning restore CS0618 
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
    
    #pragma warning disable CS0618 
    [TestCase(IncomeBand.GreaterThan31000)] //Test case tests backwards compatibility with obsolete bands
    [TestCase(IncomeBand.GreaterThan34500)] //Test case tests backwards compatibility with obsolete bands
    [TestCase(IncomeBand.GreaterThan36000)]
    #pragma warning restore CS0618 
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
    
    #pragma warning disable CS0618 
    [TestCase(IncomeBand.UnderOrEqualTo31000, true)] //Test case tests backwards compatibility with obsolete bands
    [TestCase(IncomeBand.UnderOrEqualTo31000, false)] //Test case tests backwards compatibility with obsolete bands
    [TestCase(IncomeBand.UnderOrEqualTo34500, true)] //Test case tests backwards compatibility with obsolete bands
    [TestCase(IncomeBand.UnderOrEqualTo34500, false)] //Test case tests backwards compatibility with obsolete bands
    [TestCase(IncomeBand.UnderOrEqualTo36000, true)]
    [TestCase(IncomeBand.UnderOrEqualTo36000, false)]
    #pragma warning restore CS0618 
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
    
    #pragma warning disable CS0618 
    [TestCase(IncomeBand.GreaterThan34500)] //Test case tests backwards compatibility with obsolete bands
    [TestCase(IncomeBand.UnderOrEqualTo34500)] //Test case tests backwards compatibility with obsolete bands
    #pragma warning restore CS0618 
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
    
    [TestCase(IncomeBand.GreaterThan36000)]
    [TestCase(IncomeBand.UnderOrEqualTo36000)]
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
