using FluentAssertions;
using NUnit.Framework;
using WhlgPublicWebsite.BusinessLogic.Extensions;

namespace Tests.BusinessLogic.Extensions;

[TestFixture]
public class StringExtensionTests
{
    [TestCase("M1 2AB")]   // Format 1
    [TestCase("M60 2AB")]  // Format 2
    [TestCase("CR2 2AB")]  // Format 3
    [TestCase("DN55 2AB")] // Format 4
    [TestCase("W1P 2AB")]  // Format 5
    [TestCase("EC1A 2AB")] // Format 6
    [TestCase("   M1 2AB   ")] // Leading and trailing whitespace is ignored
    [TestCase("M1    2AB")] // Extra internal whitespace is ignored
    [TestCase("m1 2ab")]   // Lower case is accepted
    [TestCase("M12AB")]   // Missing space is accepted
    public void IsValidUkPostcode_CalledWithValidPostcode_ReturnsTrue(string postcode)
    {
        // Act
        var result = postcode.IsValidUkPostcodeFormat();
        
        // Assert
        result.Should().BeTrue();
    }
    
    [TestCase("M12 AB")]   // Space in the wrong place
    [TestCase("M1 AB")]  // Inward postcode missing number
    [TestCase("M1 2A")]  // Inward postcode missing letter
    [TestCase("1M1 2A")]  // Outward postcode starting with a number
    [TestCase("M123 2AB")]  // Outward postcode with three numbers
    [TestCase("ABC1 2AB")] // Outward postcode with three letters at the start
    [TestCase("W12P 2AB")]  // Outward postcode with two internal numbers
    public void IsValidUkPostcode_CalledWithInvalidPostcode_ReturnsFalse(string postcode)
    {
        // Act
        var result = postcode.IsValidUkPostcodeFormat();
        
        // Assert
        result.Should().BeFalse();
    }
    
    [TestCase("M1 2AB", "M1 2AB")]  // Unchanged
    [TestCase("   M1 2AB   ", "M1 2AB")] // Leading and trailing whitespace
    [TestCase("M1    2AB", "M1 2AB")] // Extra internal whitespace
    [TestCase("m1 2ab", "M1 2AB")]   // Lower case
    [TestCase("M12AB", "M1 2AB")]   // Missing space
    public void NormaliseUkPostcode_CalledWithValidPostcode_ReturnsPostcode(string inputPostcode, string normalisedPostcode)
    {
        // Act
        var result = inputPostcode.NormaliseToUkPostcodeFormat();
        
        // Assert
        result.Should().Be(normalisedPostcode);
    }
    
    [TestCase("M12 AB")]   // Space in the wrong place
    [TestCase("M1 AB")]  // Inward postcode missing number
    [TestCase("M1 2A")]  // Inward postcode missing letter
    [TestCase("1M1 2A")]  // Outward postcode starting with a number
    [TestCase("M123 2AB")]  // Outward postcode with three numbers
    [TestCase("ABC1 2AB")] // Outward postcode with three letters at the start
    [TestCase("W12P 2AB")]  // Outward postcode with two internal numbers
    public void NormaliseUkPostcode_CalledWithInvalidPostcode_ReturnsNull(string inputPostcode)
    {
        // Act
        var result = inputPostcode.NormaliseToUkPostcodeFormat();
        
        // Assert
        result.Should().BeNull();
    }
}
