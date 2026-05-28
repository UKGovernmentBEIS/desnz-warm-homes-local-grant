using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using WhlgPublicWebsite.BusinessLogic.Services.EligiblePostcode;

namespace Tests.BusinessLogic.Services;

[TestFixture]
public class EligiblePostcodeServiceTests
{
    [SetUp]
    public void Setup()
    {
        logger = new NullLogger<EligiblePostcodeService>();

        var cache = new EligiblePostcodeListCache();
        underTest = new EligiblePostcodeService(cache, logger);
    }

    private ILogger<EligiblePostcodeService> logger;
    private EligiblePostcodeService underTest;

    [TestCase("BN88 1ZT")]
    [TestCase("YO24 3LY")]
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
        var result = underTest.IsEligiblePostcode("AL1 1AG");

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

    [TestCase("BN99 9GA", false, Description = "Only in old file")]
    [TestCase("N8 7JL", true, Description = "Only in new 2025 file")]
    [TestCase("YO24 3LY", true, Description = "In both files")]
    public void IsEligiblePostcode_UsesImd2025Postcodes(string postcode, bool expectedResult)
    {
        underTest.IsEligiblePostcode(postcode).Should().Be(expectedResult);
    }
}