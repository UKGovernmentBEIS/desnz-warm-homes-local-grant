using FluentAssertions;
using NUnit.Framework;
using WhlgPublicWebsite.Helpers;

namespace Tests.Website.Helpers;

[TestFixture]
public class ValidEmailAddressAttributeTests
{
    private ValidEmailAddressAttribute underTest;

    [SetUp]
    public void Setup()
    {
        underTest = new ValidEmailAddressAttribute();
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("name@example.com")]
    [TestCase("user.name+tag@example.co.uk")]
    public void IsValid_WhenEmailIsAcceptable_ReturnsTrue(string email)
    {
        underTest.IsValid(email).Should().BeTrue();
    }

    [TestCase("name@example,com")]
    [TestCase("name@exam,ple.com")]
    public void IsValid_WhenEmailContainsComma_ReturnsFalse(string email)
    {
        underTest.IsValid(email).Should().BeFalse();
    }

    [TestCase("name @example.com")]
    [TestCase("name@ example.com")]
    [TestCase("name@example .com")]
    [TestCase(" name@example.com")]
    [TestCase("name@example.com ")]
    [TestCase("name@exam ple.com")]
    public void IsValid_WhenEmailContainsWhitespace_ReturnsFalse(string email)
    {
        underTest.IsValid(email).Should().BeFalse();
    }

    [TestCase("not-an-email")]
    [TestCase("missing-at.example.com")]
    public void IsValid_WhenEmailIsOtherwiseInvalid_ReturnsFalse(string email)
    {
        underTest.IsValid(email).Should().BeFalse();
    }
}
