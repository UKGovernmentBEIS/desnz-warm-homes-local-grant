using System;
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

        var cache = new EligiblePostcodeListCache(new EligiblePostcodeImdFileChecker());
        underTest = new EligiblePostcodeService(cache, logger);
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

    // TODO DESNZ-2197: Remove this once we have moved entirely to the IMD2025 postcodes.
    // BN99 9GA: only in the original file
    // N8 7JL: only in the IMD2025 file
    // YO24 3LY: in both files
    [TestCase("BN99 9GA", true, Description = "Only in original file")]
    [TestCase("N8 7JL", false, Description = "Only in IMD2025 file")]
    [TestCase("YO24 3LY", true, Description = "In both files")]
    public void IsEligiblePostcode_BeforeApril2026_UsesOriginalPostcodes(string postcode, bool expectedResult)
    {
        var service = CreateServiceWithDate(new DateTime(2026, 3, 31));

        service.IsEligiblePostcode(postcode).Should().Be(expectedResult);
    }

    [TestCase("BN99 9GA", false, Description = "Only in original file")]
    [TestCase("N8 7JL", true, Description = "Only in IMD2025 file")]
    [TestCase("YO24 3LY", true, Description = "In both files")]
    public void IsEligiblePostcode_OnOrAfterApril2026_UsesImd2025Postcodes(string postcode, bool expectedResult)
    {
        var service = CreateServiceWithDate(new DateTime(2026, 4, 1));

        service.IsEligiblePostcode(postcode).Should().Be(expectedResult);
    }

    private EligiblePostcodeService CreateServiceWithDate(DateTime date)
    {
        var imdChecker = new EligiblePostcodeImdFileChecker(() => date);
        var cache = new EligiblePostcodeListCache(imdChecker);
        return new EligiblePostcodeService(cache, logger);
    }
}