using FluentAssertions;
using WhlgPublicWebsite.BusinessLogic.Services.EligiblePostcode;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace Tests.BusinessLogic.Services;

[TestFixture]
public class EligiblePostcodeServiceTests
{
    [SetUp]
    public void Setup()
    {
        logger = new NullLogger<EligiblePostcodeService>();

        var eligiblePostcodeListCache = new EligiblePostcodeListCache();
        underTest = new EligiblePostcodeService(eligiblePostcodeListCache, logger);
    }

    private ILogger<EligiblePostcodeService> logger;
    private EligiblePostcodeService underTest;

    [TestCase("BN99 9GA")]
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
        var result = underTest.IsEligiblePostcode("AL1 2AP");

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