using System;
using FluentAssertions;
using WhlgPublicWebsite.BusinessLogic.Services.S3ReferralFileKeyGenerator;
using NUnit.Framework;

namespace Tests.BusinessLogic.Services;

[TestFixture]
public class S3ReferralFileKeyGeneratorTests
{
    private S3ReferralFileKeyGenerator underTest;

    [SetUp]
    public void Setup()
    {
        underTest = new S3ReferralFileKeyGenerator();
    }
    
    [TestCase("114", 1, 2020, "114/2020_01.csv")]
    [TestCase("114", 10, 2020, "114/2020_10.csv")]
    public void GetS3Key_WhenCalledWithValidData_CreatesCorrectKey(string custodianCode, int month, int year, string expectedKey)
    {
        // Act
        var key = underTest.GetS3Key(custodianCode, month, year);

        // Assert
        key.Should().Be(expectedKey);
    }
    
    [TestCase("invalid", 1, 2020)] // Invalid custodian code
    [TestCase("114", 13, 2020)] // Month is 13
    [TestCase("114", 1, 999)] // Year is less than 4 digits
    [TestCase("114", 0, 2020)] // Month is 0
    [TestCase("114", 1, 10000)] // Year is greater than 4 digits
    public void GetS3Key_WhenCalledWithInvalidData_ThrowsException(string custodianCode, int month, int year)
    {
        // Act and Assert
        Assert.Throws<ArgumentException>(() => underTest.GetS3Key(custodianCode, month, year));
    }
}
