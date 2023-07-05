using System;
using FluentAssertions;
using NUnit.Framework;
using HerPublicWebsite.BusinessLogic.Models;
using HerPublicWebsite.BusinessLogic.Models.Enums;

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
            HasGasBoiler = HasGasBoiler.Yes,
            Country = Country.England,
            OwnershipStatus = OwnershipStatus.Landlord,
            EpcDetailsAreCorrect = EpcConfirmation.Yes,
            IsLsoaProperty = true,
            IncomeBand = IncomeBand.UnderOrEqualTo31000,
        };

        // Act
        var report = new AnonymisedReport(questionnaire);

        // Assert
        report.PostcodeFirstHalf.Should().Be(expectedFirstHalf);
    }
}
