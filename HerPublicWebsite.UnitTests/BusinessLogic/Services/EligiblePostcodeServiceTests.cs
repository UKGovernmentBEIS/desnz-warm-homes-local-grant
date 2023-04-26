using FluentAssertions;
using NUnit.Framework;
using HerPublicWebsite.BusinessLogic.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Tests.BusinessLogic.Services;

[TestFixture]
public class EligiblePostcodeServiceTests
{
    private ILogger<EligiblePostcodeService> logger;
    private EligiblePostcodeService underTest;

    [SetUp]
    public void Setup()
    {
        logger = new NullLogger<EligiblePostcodeService>();

        underTest = new EligiblePostcodeService(logger);
    }
    
    [TestCase("AL1 2AP")]
    [TestCase("YO8 9SF")]
    public void IsEligiblePostcode_CalledWithEligiblePostcode_ReturnsTrue(string postcode)
    {
        // Act
        var result = underTest.IsEligiblePostcode(postcode);

        // Assert
        result.Should().BeTrue();
    }
    
    [Test]
    public void IsEligiblePostcode_CalledWithIneligiblePostcode_ReturnsFalse()
    {
        // Act
        var result = underTest.IsEligiblePostcode("NW5 1TL");

        // Assert
        result.Should().BeFalse();
    }
    
    [Test]
    public void IsEligiblePostcode_CalledWithInvalidPostcode_ReturnsFalse()
    {
        // Act
        var result = underTest.IsEligiblePostcode("NOT_A_POSTCODE");

        // Assert
        result.Should().BeFalse();
    }
}
