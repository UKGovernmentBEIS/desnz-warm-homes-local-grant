using System;
using FluentAssertions;
using NUnit.Framework;
using WhlgPublicWebsite.BusinessLogic.Models;
using WhlgPublicWebsite.BusinessLogic.Models.Enums;

namespace Tests.BusinessLogic.Models;

[TestFixture]
public class AnonymisedReportTests
{
    [TestCase("SW1A 2AA", "SW1A")] // Normal, prenormalised postcode
    [TestCase("THISISNOTAVALIDPOSTCODE", null)] // Invalid postcode
    [TestCase("w21jb", "W2")] // Valid postcode, not already normalised
    public void PostcodeFirstHalf_IsGeneratedFromPostcode(string postcode, string expectedFirstHalf)
    {
        // Arrange
        var questionnaire = new Questionnaire
        {
            AddressPostcode = postcode,
            Country = Country.England,
            OwnershipStatus = OwnershipStatus.Landlord,
            EpcDetailsAreCorrect = EpcConfirmation.Yes,
            IsLsoaProperty = true,
            IncomeBand = IncomeBand.UnderOrEqualTo36000
        };

        // Act
        var report = new AnonymisedReport(questionnaire);

        // Assert
        report.PostcodeFirstHalf.Should().Be(expectedFirstHalf);
    }
}
