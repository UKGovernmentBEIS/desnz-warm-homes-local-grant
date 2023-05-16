using System;
using FluentAssertions;
using HerPublicWebsite.BusinessLogic.Models;
using HerPublicWebsite.BusinessLogic.Models.Enums;
using NUnit.Framework;

namespace Tests.BusinessLogic.Models;

[TestFixture]
public class QuestionnaireTests
{
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
}
