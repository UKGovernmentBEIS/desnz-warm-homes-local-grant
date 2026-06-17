using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using Tests.Builders;
using WhlgPublicWebsite.BusinessLogic.Models;
using WhlgPublicWebsite.BusinessLogic.Models.Enums;
using WhlgPublicWebsite.BusinessLogic.Services.CsvFileCreator;

namespace Tests.BusinessLogic.Services;

// Note that in these tests we need to specify the line breaks explicitly as the CSV library always uses \r\n. If we use
// C#'s @"" strings in the expected values then the line breaks will change depending on the platform the test is built on.
[TestFixture]
public class CsvFileCreatorTests
{
    private const string UnknownCustodianCode = "not-a-valid-custodian-code";

    [Test]
    public void CreateReferralRequestFileDataForS3_CalledWithReferralRequest_GeneratesExpectedFileData()
    {
        // Arrange
        var underTest = new CsvFileCreator(new NullLogger<CsvFileCreator>());
        var referralRequest = new ReferralRequestBuilder(1).Build();
        var referralRequests = new List<ReferralRequest> { referralRequest };

        // Act
        var data = underTest.CreateReferralRequestFileDataForS3(referralRequests);

        // Assert
        var reader = new StreamReader(data, Encoding.UTF8);
        reader.ReadToEnd().Should().Be(
            "Referral date,Referral code,Name,Email,Telephone,Address1,Address2,Town,County,Postcode,UPRN,EPC Band,EPC confirmed by homeowner,EPC Lodgement Date,Household income band,Is eligible postcode,Tenure\r\n" +
            "2023-01-01 13:00:01,DummyCode00001,Full Name1,contact1@example.com,00001 123456,Address 1 line 1,Address 1 line 2,Town1,County1,AL01 1RS,100 111 222 001,E,,2023-01-01 15:00:01,\"£36,000 or less\",no,Owner\r\n");
    }


#pragma warning disable CS0618 // Obsolete IncomeBands used to preserve backwards-compatibility
    [TestCase(IncomeBand.GreaterThan31000, "£31k or above")]
    [TestCase(IncomeBand.GreaterThan34500, "£34.5k or above")]
    [TestCase(IncomeBand.UnderOrEqualTo34500, "Below £34.5k")]
    [TestCase(IncomeBand.UnderOrEqualTo31000, "Below £31k")]
#pragma warning restore CS0618
    [TestCase(IncomeBand.GreaterThan36000, "\"More than £36,000\"")]
    [TestCase(IncomeBand.UnderOrEqualTo36000, "\"£36,000 or less\"")]
    public void
        CreateReferralRequestFileDataForS3_CalledWithReferralRequestWithIncomeAbove31k_GeneratesExpectedFileData(
            IncomeBand incomeBand, string expectedValue)
    {
        // Arrange
        var underTest = new CsvFileCreator(new NullLogger<CsvFileCreator>());
        var referralRequest = new ReferralRequestBuilder(1).WithIncomeBand(incomeBand).Build();
        var referralRequests = new List<ReferralRequest> { referralRequest };

        // Act
        var data = underTest.CreateReferralRequestFileDataForS3(referralRequests);

        // Assert
        var reader = new StreamReader(data, Encoding.UTF8);
        reader.ReadToEnd().Should().Be(
            "Referral date,Referral code,Name,Email,Telephone,Address1,Address2,Town,County,Postcode,UPRN,EPC Band,EPC confirmed by homeowner,EPC Lodgement Date,Household income band,Is eligible postcode,Tenure\r\n" +
            $"2023-01-01 13:00:01,DummyCode00001,Full Name1,contact1@example.com,00001 123456,Address 1 line 1,Address 1 line 2,Town1,County1,AL01 1RS,100 111 222 001,E,,2023-01-01 15:00:01,{expectedValue},no,Owner\r\n");
    }

    [Test]
    public void CreateReferralRequestFileDataForS3_CalledWithReferralRequestWithUnsureEpc_GeneratesExpectedFileData()
    {
        // Arrange
        var underTest = new CsvFileCreator(new NullLogger<CsvFileCreator>());
        var referralRequest = new ReferralRequestBuilder(1).WithEpcConfirmation(EpcConfirmation.Unknown).Build();
        var referralRequests = new List<ReferralRequest> { referralRequest };

        // Act
        var data = underTest.CreateReferralRequestFileDataForS3(referralRequests);

        // Assert
        var reader = new StreamReader(data, Encoding.UTF8);
        reader.ReadToEnd().Should().Be(
            "Referral date,Referral code,Name,Email,Telephone,Address1,Address2,Town,County,Postcode,UPRN,EPC Band,EPC confirmed by homeowner,EPC Lodgement Date,Household income band,Is eligible postcode,Tenure\r\n" +
            "2023-01-01 13:00:01,DummyCode00001,Full Name1,contact1@example.com,00001 123456,Address 1 line 1,Address 1 line 2,Town1,County1,AL01 1RS,100 111 222 001,E,Homeowner unsure,2023-01-01 15:00:01,\"£36,000 or less\",no,Owner\r\n");
    }

    [Test]
    public void CreateReferralRequestFileDataForS3_CalledWithMultipleReferralRequests_GeneratesExpectedFileData()
    {
        // Arrange
        var underTest = new CsvFileCreator(new NullLogger<CsvFileCreator>());
        var referralRequest1 = new ReferralRequestBuilder(1).Build();
        var referralRequest2 = new ReferralRequestBuilder(2).Build();
        var referralRequest3 = new ReferralRequestBuilder(3).Build();
        var referralRequests = new List<ReferralRequest> { referralRequest1, referralRequest2, referralRequest3 };

        // Act
        var data = underTest.CreateReferralRequestFileDataForS3(referralRequests);

        // Assert
        var reader = new StreamReader(data, Encoding.UTF8);
        reader.ReadToEnd().Should().Be(
            "Referral date,Referral code,Name,Email,Telephone,Address1,Address2,Town,County,Postcode,UPRN,EPC Band,EPC confirmed by homeowner,EPC Lodgement Date,Household income band,Is eligible postcode,Tenure\r\n" +
            "2023-01-01 13:00:01,DummyCode00001,Full Name1,contact1@example.com,00001 123456,Address 1 line 1,Address 1 line 2,Town1,County1,AL01 1RS,100 111 222 001,E,,2023-01-01 15:00:01,\"£36,000 or less\",no,Owner\r\n" +
            "2023-01-01 13:00:02,DummyCode00002,Full Name2,contact2@example.com,00002 123456,Address 2 line 1,Address 2 line 2,Town2,County2,AL02 1RS,100 111 222 002,E,,2023-01-01 15:00:02,\"£36,000 or less\",no,Owner\r\n" +
            "2023-01-01 13:00:03,DummyCode00003,Full Name3,contact3@example.com,00003 123456,Address 3 line 1,Address 3 line 2,Town3,County3,AL03 1RS,100 111 222 003,E,,2023-01-01 15:00:03,\"£36,000 or less\",no,Owner\r\n");
    }

    [TestCase("comma,separated,value", "\"comma,separated,value\"")]
    [TestCase("double\"quotes", "\"double\"\"quotes\"")]
    [TestCase("=Formula()", "Formula()")]
    [TestCase("+441234567890", "441234567890")]
    public void CreateReferralRequestFileDataForS3_CalledWithSpecialCharacters_GeneratesEscapedFileData(
        string nameInput,
        string expectedOutput)
    {
        // Arrange
        var underTest = new CsvFileCreator(new NullLogger<CsvFileCreator>());
        var referralRequest = new ReferralRequestBuilder(1).WithFullName(nameInput).Build();
        var referralRequests = new List<ReferralRequest> { referralRequest };

        // Act
        var data = underTest.CreateReferralRequestFileDataForS3(referralRequests);

        // Assert
        var reader = new StreamReader(data, Encoding.UTF8);
        reader.ReadToEnd().Should().Be(
            "Referral date,Referral code,Name,Email,Telephone,Address1,Address2,Town,County,Postcode,UPRN,EPC Band,EPC confirmed by homeowner,EPC Lodgement Date,Household income band,Is eligible postcode,Tenure\r\n" +
            $"2023-01-01 13:00:01,DummyCode00001,{expectedOutput},contact1@example.com,00001 123456,Address 1 line 1,Address 1 line 2,Town1,County1,AL01 1RS,100 111 222 001,E,,2023-01-01 15:00:01,\"£36,000 or less\",no,Owner\r\n");
    }

    [Test]
    public void CreateReferralRequestFileDataForS3_CalledWithReferralRequest_IncludesBOMInTheMemoryStream()
    {
        // Arrange
        var underTest = new CsvFileCreator(new NullLogger<CsvFileCreator>());
        var referralRequest = new ReferralRequestBuilder(1).Build();
        var referralRequests = new List<ReferralRequest> { referralRequest };

        // Act
        var data = underTest.CreateReferralRequestFileDataForS3(referralRequests);

        // Assert
        ContainsBom(data).Should().BeTrue();
    }
    
    [Test]
    public void CreateReferralRequestFileDataForS3_CalledWithReferralRequestWithEpcConfirmedYes_GeneratesExpectedFileData()
    {
        // Arrange
        var underTest = new CsvFileCreator(new NullLogger<CsvFileCreator>());
        var referralRequest = new ReferralRequestBuilder(1).WithEpcConfirmation(EpcConfirmation.Yes).Build();
        var referralRequests = new List<ReferralRequest> { referralRequest };

        // Act
        var data = underTest.CreateReferralRequestFileDataForS3(referralRequests);

        // Assert
        var reader = new StreamReader(data, Encoding.UTF8);
        reader.ReadToEnd().Should().Be(
            "Referral date,Referral code,Name,Email,Telephone,Address1,Address2,Town,County,Postcode,UPRN,EPC Band,EPC confirmed by homeowner,EPC Lodgement Date,Household income band,Is eligible postcode,Tenure\r\n");
    }

    [Test]
    public void CreateReferralRequestFileDataForS3_WhenRowThrows_LogsErrorWithSkippedRow()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<CsvFileCreator>>();
        var underTest = new CsvFileCreator(mockLogger.Object);
        var validRequest = new ReferralRequestBuilder(1).Build();
        var faultyRequest = new ReferralRequestBuilder(2).WithEpcConfirmation(EpcConfirmation.Yes).Build();
        var referralRequests = new List<ReferralRequest> { validRequest, faultyRequest };

        // Act
        underTest.CreateReferralRequestFileDataForS3(referralRequests);

        // Assert
        VerifySkippingRowLoggedOnce(mockLogger, $"ReferralCode={faultyRequest.ReferralCode}");
    }

    [Test]
    public void CreateReferralRequestOverviewFileDataForS3_WhenRowThrows_LogsErrorWithSkippedRow()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<CsvFileCreator>>();
        var underTest = new CsvFileCreator(mockLogger.Object);
        var validRequest = new ReferralRequestBuilder(1).WithCustodianCode("114").Build();
        var faultyRequest = new ReferralRequestBuilder(2).WithCustodianCode(UnknownCustodianCode).Build();
        var referralRequests = new List<ReferralRequest> { validRequest, faultyRequest };

        // Act
        underTest.CreateReferralRequestOverviewFileDataForS3(referralRequests);

        // Assert
        VerifySkippingRowLoggedOnce(mockLogger, $"ReferralCode={faultyRequest.ReferralCode}");
    }

    [Test]
    public void CreateLocalAuthorityReferralRequestFollowUpFileDataForS3_WhenRowThrows_LogsErrorWithSkippedRow()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<CsvFileCreator>>();
        var underTest = new CsvFileCreator(mockLogger.Object);
        var validRequest = new ReferralRequestBuilder(1).WithCustodianCode("114").Build();
        var faultyRequest = new ReferralRequestBuilder(2).WithCustodianCode(UnknownCustodianCode).Build();
        var referralRequests = new List<ReferralRequest> { validRequest, faultyRequest };

        // Act
        underTest.CreateLocalAuthorityReferralRequestFollowUpFileDataForS3(referralRequests);

        // Assert
        VerifySkippingRowLoggedOnce(mockLogger, $"CustodianCode={UnknownCustodianCode}");
    }

    [Test]
    public void CreatePendingReferralRequestFileDataForS3_WhenRowThrows_LogsErrorWithSkippedRow()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<CsvFileCreator>>();
        var underTest = new CsvFileCreator(mockLogger.Object);
        var validRequest = new ReferralRequestBuilder(1).WithCustodianCode("114").Build();
        var faultyRequest = new ReferralRequestBuilder(2).WithCustodianCode(UnknownCustodianCode).Build();
        var referralRequests = new List<ReferralRequest> { validRequest, faultyRequest };

        // Act
        underTest.CreatePendingReferralRequestFileDataForS3(referralRequests);

        // Assert
        VerifySkippingRowLoggedOnce(mockLogger, $"ReferralCode={faultyRequest.ReferralCode}");
    }

    [Test]
    public void CreatePerMonthLocalAuthorityReferralStatisticsForConsole_WhenRowThrows_LogsErrorWithSkippedRow()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<CsvFileCreator>>();
        var underTest = new CsvFileCreator(mockLogger.Object);
        var validRequest = new ReferralRequestBuilder(1).WithCustodianCode("114").Build();
        var faultyRequest = new ReferralRequestBuilder(2).WithCustodianCode(UnknownCustodianCode).Build();
        var referralRequests = new List<ReferralRequest> { validRequest, faultyRequest };

        // Act
        underTest.CreatePerMonthLocalAuthorityReferralStatisticsForConsole(referralRequests);

        // Assert
        VerifySkippingRowLoggedOnce(mockLogger, $"CustodianCode={UnknownCustodianCode}");
    }

    [Test]
    public void CreateReferralRequestOverviewFileDataForS3_CalledWithReferralRequest_GeneratesExpectedFileData()
    {
        // Arrange
        var underTest = new CsvFileCreator(new NullLogger<CsvFileCreator>());
        var referralRequest = new ReferralRequestBuilder(1).WithCustodianCode("114").Build();
        var localAuthority = GetLocalAuthorityDetails(referralRequest);
        var referralRequests = new List<ReferralRequest> { referralRequest };

        // Act
        var data = underTest.CreateReferralRequestOverviewFileDataForS3(referralRequests);

        // Assert
        var reader = new StreamReader(data, Encoding.UTF8);
        reader.ReadToEnd().Should().Be(
            "Consortium,Local Authority,Referral Code\r\n" +
            $"{localAuthority.Consortium},{localAuthority.Name},{referralRequest.ReferralCode}\r\n");
    }

    [Test]
    public void
        CreateReferralRequestOverviewFileDataForS3_CalledWithReferralRequestFromNullConsortium_GeneratesExpectedFileData()
    {
        // Arrange
        var underTest = new CsvFileCreator(new NullLogger<CsvFileCreator>());
        var referralRequest = new ReferralRequestBuilder(1).WithCustodianCode("9052")
            .WithFollowUp(new ReferralRequestFollowUpBuilder(1)).Build();
        var localAuthority = GetLocalAuthorityDetails(referralRequest);
        var referralRequests = new List<ReferralRequest> { referralRequest };

        // Act
        var data = underTest.CreateReferralRequestOverviewFileDataForS3(referralRequests);

        // Assert
        var reader = new StreamReader(data, Encoding.UTF8);
        reader.ReadToEnd().Should().Be(
            "Consortium,Local Authority,Referral Code\r\n" +
            $"{localAuthority.Consortium},{localAuthority.Name},{referralRequest.ReferralCode}\r\n");
    }

    [Test]
    public void CreateReferralRequestOverviewFileDataForS3_CalledWithReferralRequest_IncludesBOMInTheMemoryStream()
    {
        // Arrange
        var underTest = new CsvFileCreator(new NullLogger<CsvFileCreator>());
        var referralRequest = new ReferralRequestBuilder(1).Build();
        var referralRequests = new List<ReferralRequest> { referralRequest };

        // Act
        var data = underTest.CreateReferralRequestOverviewFileDataForS3(referralRequests);

        // Assert
        ContainsBom(data).Should().BeTrue();
    }

    [Test]
    public void
        CreateLocalAuthorityReferralRequestFollowUpFileDataForS3_CalledWithReferralRequest_GeneratesExpectedFileData()
    {
        // Arrange
        var underTest = new CsvFileCreator(new NullLogger<CsvFileCreator>());
        var referralRequest1 = new ReferralRequestBuilder(1).WithWrittenToCsv(false).WithCustodianCode("114")
            .WithFollowUp(new ReferralRequestFollowUpBuilder(1).WithWasFollowedUp(null)).Build();
        var referralRequest2 = new ReferralRequestBuilder(2).WithWrittenToCsv(false).WithCustodianCode("114")
            .WithFollowUp(new ReferralRequestFollowUpBuilder(2).WithWasFollowedUp(false)).Build();
        var referralRequest3 = new ReferralRequestBuilder(3).WithWrittenToCsv(true).WithCustodianCode("121")
            .WithFollowUp(new ReferralRequestFollowUpBuilder(3).WithWasFollowedUp(true)).Build();
        var referralRequest4 = new ReferralRequestBuilder(4).WithWrittenToCsv(true).WithCustodianCode("121")
            .WithFollowUp(new ReferralRequestFollowUpBuilder(4).WithWasFollowedUp(false)).Build();
        var referralRequest5 = new ReferralRequestBuilder(5).WithWrittenToCsv(false).WithCustodianCode("121")
            .WithFollowUp(new ReferralRequestFollowUpBuilder(5).WithWasFollowedUp(true)).Build();
        var referralRequest6 = new ReferralRequestBuilder(6).WithWrittenToCsv(false).WithCustodianCode("9052")
            .WithFollowUp(new ReferralRequestFollowUpBuilder(6).WithWasFollowedUp(false)).Build();
        var referralRequest7 = new ReferralRequestBuilder(7).WithWrittenToCsv(true).WithCustodianCode("9052")
            .WithFollowUp(new ReferralRequestFollowUpBuilder(7).WithWasFollowedUp(true)).Build();
        var referralRequest8 = new ReferralRequestBuilder(8).WithWrittenToCsv(true).WithCustodianCode("9052")
            .WithFollowUp(new ReferralRequestFollowUpBuilder(8).WithWasFollowedUp(false)).Build();

        var referralRequests = new List<ReferralRequest>
        {
            referralRequest1, referralRequest2, referralRequest3, referralRequest4, referralRequest5, referralRequest6,
            referralRequest7, referralRequest8
        };
        var localAuthority1 = GetLocalAuthorityDetails(referralRequest1);
        var localAuthority3 = GetLocalAuthorityDetails(referralRequest3);
        var localAuthority6 = GetLocalAuthorityDetails(referralRequest6);
        var today = DateTime.Today.ToString("dd-MMM");

        // Act
        var data = underTest.CreateLocalAuthorityReferralRequestFollowUpFileDataForS3(referralRequests);

        // Assert
        var reader = new StreamReader(data, Encoding.UTF8);
        reader.ReadToEnd().Should().Be(
            "SLA Report Date,LA,LA Number of Referrals Not Downloaded,LA Percentage of Referrals Not Downloaded," +
            "LA Number of Referrals Not Contacted,LA Percentage of Referrals Not Contacted,LA Number of Referrals Responded to email,LA Percentage of Referrals Responded to email\r\n" +
            $"{today},{localAuthority1.Name},2,100,1,50,1,50\r\n" +
            $"{today},{localAuthority3.Name},1,33.333333333333336,1,33.333333333333336,3,100\r\n" +
            $"{today},{localAuthority6.Name},1,33.333333333333336,2,66.66666666666667,3,100\r\n"
        );
    }

    [Test]
    public void
        CreateLocalAuthorityReferralRequestFollowUpFileDataForS3_CalledWithReferralRequest_IncludesBOMInTheMemoryStream()
    {
        // Arrange
        var underTest = new CsvFileCreator(new NullLogger<CsvFileCreator>());
        var referralRequest = new ReferralRequestBuilder(1).Build();
        var referralRequests = new List<ReferralRequest> { referralRequest };

        // Act
        var data = underTest.CreateLocalAuthorityReferralRequestFollowUpFileDataForS3(referralRequests);

        // Assert
        ContainsBom(data).Should().BeTrue();
    }

    [Test]
    public void
        CreateConsortiumReferralRequestFollowUpFileDataForS3_CalledWithReferralRequest_GeneratesExpectedFileData()
    {
        // Arrange
        var underTest = new CsvFileCreator(new NullLogger<CsvFileCreator>());
        var referralRequest1 = new ReferralRequestBuilder(1).WithWrittenToCsv(false).WithCustodianCode("114")
            .WithFollowUp(new ReferralRequestFollowUpBuilder(1).WithWasFollowedUp(null)).Build();
        var referralRequest2 = new ReferralRequestBuilder(2).WithWrittenToCsv(false).WithCustodianCode("114")
            .WithFollowUp(new ReferralRequestFollowUpBuilder(2).WithWasFollowedUp(false)).Build();
        var referralRequest3 = new ReferralRequestBuilder(3).WithWrittenToCsv(true).WithCustodianCode("121")
            .WithFollowUp(new ReferralRequestFollowUpBuilder(3).WithWasFollowedUp(true)).Build();
        var referralRequest4 = new ReferralRequestBuilder(4).WithWrittenToCsv(true).WithCustodianCode("121")
            .WithFollowUp(new ReferralRequestFollowUpBuilder(4).WithWasFollowedUp(false)).Build();
        var referralRequest5 = new ReferralRequestBuilder(5).WithWrittenToCsv(false).WithCustodianCode("121")
            .WithFollowUp(new ReferralRequestFollowUpBuilder(5).WithWasFollowedUp(true)).Build();
        var referralRequest6 = new ReferralRequestBuilder(6).WithWrittenToCsv(false).WithCustodianCode("9052")
            .WithFollowUp(new ReferralRequestFollowUpBuilder(6).WithWasFollowedUp(false)).Build();
        var referralRequest7 = new ReferralRequestBuilder(7).WithWrittenToCsv(true).WithCustodianCode("9052")
            .WithFollowUp(new ReferralRequestFollowUpBuilder(7).WithWasFollowedUp(true)).Build();
        var referralRequest8 = new ReferralRequestBuilder(8).WithWrittenToCsv(true).WithCustodianCode("9052")
            .WithFollowUp(new ReferralRequestFollowUpBuilder(8).WithWasFollowedUp(false)).Build();
        var referralRequest9 = new ReferralRequestBuilder(9).WithWrittenToCsv(true).WithCustodianCode("2205")
            .WithFollowUp(new ReferralRequestFollowUpBuilder(9).WithWasFollowedUp(false)).Build();

        var referralRequests = new List<ReferralRequest>
        {
            referralRequest1, referralRequest2, referralRequest3, referralRequest4, referralRequest5, referralRequest6,
            referralRequest7, referralRequest8, referralRequest9
        };
        var consortium = GetLocalAuthorityDetails(referralRequest1).Consortium;

        var today = DateTime.Today.ToString("dd-MMM");
        // Act
        var data = underTest.CreateConsortiumReferralRequestFollowUpFileDataForS3(referralRequests);

        // Assert
        var reader = new StreamReader(data, Encoding.UTF8);
        reader.ReadToEnd().Should().Be(
            "SLA Report Date,Consortium,Consortium All Referrals Downloaded,Consortium Number of Referrals Not Downloaded," +
            "Consortium Percentage of Referrals Not Downloaded,Consortium All Referrals Contacted," +
            "Consortium Number of Referrals Not Contacted,Consortium Percentage of Referrals Not Contacted\r\n" +
            $"{today},{consortium},False,3,60,False,2,40\r\n"
        ); // Stats for 9052 and 2205 with no Consortium name should not appear as LAs with no Consortium should not be included
    }

    [Test]
    public void
        CreateConsortiumReferralRequestFollowUpFileDataForS3_CalledWithReferralRequest_IncludesBOMInTheMemoryStream()
    {
        // Arrange
        var underTest = new CsvFileCreator(new NullLogger<CsvFileCreator>());
        var referralRequest = new ReferralRequestBuilder(1).Build();
        var referralRequests = new List<ReferralRequest> { referralRequest };

        // Act
        var data = underTest.CreateConsortiumReferralRequestFollowUpFileDataForS3(referralRequests);

        // Assert
        ContainsBom(data).Should().BeTrue();
    }

    [Test]
    public void CreatePendingReferralRequestFileDataForS3_CalledWithReferralRequest_GeneratesExpectedFileData()
    {
        // Arrange
        var underTest = new CsvFileCreator(new NullLogger<CsvFileCreator>());
        var referralRequest1 = new ReferralRequestBuilder(1)
            .WithCustodianCode("114")
            .WithRequestDate(new DateTime(2024, 3, 5, 1, 0, 0))
            .WithReferralCode("TEST0001")
            .WithFullName("Test User 1")
            .WithEmailAddress("test1@example.com")
            .WithTelephone("111")
            .Build();
        var referralRequest2 = new ReferralRequestBuilder(6)
            .WithCustodianCode("235")
            .WithRequestDate(new DateTime(2024, 2, 5, 1, 0, 0))
            .WithReferralCode("TEST0002")
            .WithFullName("Test User 2")
            .WithEmailAddress("test2@example.com")
            .WithTelephone("")
            .Build();
        var referralRequest3 = new ReferralRequestBuilder(3)
            .WithCustodianCode("121")
            .WithRequestDate(new DateTime(2024, 1, 5, 1, 0, 0))
            .WithReferralCode("TEST0003")
            .WithFullName("Test User 3")
            .WithEmailAddress("")
            .WithTelephone("333")
            .Build();
        var localAuthority1 = GetLocalAuthorityDetails(referralRequest1);
        var localAuthority2 = GetLocalAuthorityDetails(referralRequest2);
        var localAuthority3 = GetLocalAuthorityDetails(referralRequest3);

        var referralRequests = new List<ReferralRequest> { referralRequest1, referralRequest2, referralRequest3 };

        // Act
        var data = underTest.CreatePendingReferralRequestFileDataForS3(referralRequests);

        // Assert
        var reader = new StreamReader(data, Encoding.UTF8);
        reader.ReadToEnd().Should().Be(
            "Consortium,Local Authority,Referral Date,Referral Code,Name,Email,Telephone,Local Authority Status\r\n" +
            $"{localAuthority1.Consortium},{localAuthority1.Name},2024-03-05 01:00:00,TEST0001,Test User 1,test1@example.com,111,{localAuthority1.Status}\r\n" +
            $"{localAuthority2.Consortium},{localAuthority2.Name},2024-02-05 01:00:00,TEST0002,Test User 2,test2@example.com,,{localAuthority2.Status}\r\n" +
            $"{localAuthority3.Consortium},{localAuthority3.Name},2024-01-05 01:00:00,TEST0003,Test User 3,,333,{localAuthority3.Status}\r\n");
    }

    [Test]
    public void CreatePendingReferralRequestFileDataForS3_CalledWithReferralRequest_IncludesBOMInTheMemoryStream()
    {
        // Arrange
        var underTest = new CsvFileCreator(new NullLogger<CsvFileCreator>());
        var referralRequest = new ReferralRequestBuilder(1).Build();
        var referralRequests = new List<ReferralRequest> { referralRequest };

        // Act
        var data = underTest.CreatePendingReferralRequestFileDataForS3(referralRequests);

        // Assert
        ContainsBom(data).Should().BeTrue();
    }

    [Test]
    public void
        CreatePerMonthLocalAuthorityReferralStatisticsForConsole_CalledWithReferralRequest_GeneratesExpectedFileData()
    {
        // Arrange
        var underTest = new CsvFileCreator(new NullLogger<CsvFileCreator>());
        var today = DateTime.Now;
        var requestTime1 = today;
        var requestDate1 = new DateTime(requestTime1.Year, requestTime1.Month, 5);
        var referralRequest1 = new ReferralRequestBuilder(1)
            .WithCustodianCode("114")
            .WithRequestDate(requestDate1)
            .Build();
        var requestTime2 = today.AddMonths(-3);
        var requestDate2 = new DateTime(requestTime2.Year, requestTime2.Month, 5);
        var referralRequest2 = new ReferralRequestBuilder(6)
            .WithCustodianCode("235")
            .WithRequestDate(requestDate2)
            .Build();
        var requestTime3 = today.AddMonths(-3);
        var requestDate3 = new DateTime(requestTime3.Year, requestTime3.Month, 5);
        var referralRequest3 = new ReferralRequestBuilder(3)
            .WithCustodianCode("121")
            .WithRequestDate(requestDate3)
            .Build();
        var requestTime4 = today.AddMonths(-3);
        var requestDate4 = new DateTime(requestTime4.Year, requestTime4.Month, 5);
        var referralRequest4 = new ReferralRequestBuilder(3)
            .WithCustodianCode("121")
            .WithRequestDate(requestDate4)
            .Build();
        var requestTime5 = today.AddMonths(-3);
        var requestDate5 = new DateTime(requestTime5.Year, requestTime5.Month, 5);
        var referralRequest5 = new ReferralRequestBuilder(3)
            .WithCustodianCode("121")
            .WithRequestDate(requestDate5)
            .Build();

        var referralRequests = new List<ReferralRequest>
            { referralRequest1, referralRequest2, referralRequest3, referralRequest4, referralRequest5 };
        var localAuthority1 = GetLocalAuthorityDetails(referralRequest1);
        var localAuthority2 = GetLocalAuthorityDetails(referralRequest2);
        var localAuthority3 = GetLocalAuthorityDetails(referralRequest3);

        // Act
        var data = underTest.CreatePerMonthLocalAuthorityReferralStatisticsForConsole(referralRequests);

        // Assert
        var reader = new StreamReader(data, Encoding.UTF8);
        reader.ReadToEnd().Should().Be(
            "LA Name,Consortium Name,Total WH:LG Referrals,Date of First Referral,Months Since First Referral,Referrals Per Month\r\n" +
            $"{localAuthority1.Name},{localAuthority1.Consortium},1,{requestDate1:dd/MM/yyyy},1,1\r\n" +
            $"{localAuthority2.Name},{localAuthority2.Consortium},1,{requestDate2:dd/MM/yyyy},3,0.33\r\n" +
            $"{localAuthority3.Name},{localAuthority3.Consortium},3,{requestDate3:dd/MM/yyyy},3,1\r\n");
    }

    [Test]
    public void
        CreatePerMonthLocalAuthorityReferralStatisticsForConsole_CalledWithReferralRequest_DoesNotIncludeBOMInTheMemoryStream()
    {
        // Arrange
        var underTest = new CsvFileCreator(new NullLogger<CsvFileCreator>());
        var referralRequest = new ReferralRequestBuilder(1).Build();
        var referralRequests = new List<ReferralRequest> { referralRequest };

        // Act
        var data = underTest.CreatePerMonthLocalAuthorityReferralStatisticsForConsole(referralRequests);

        // Assert
        ContainsBom(data).Should().BeFalse();
    }

    [Test]
    public void
        CreatePerMonthConsortiumReferralStatisticsForConsole_CalledWithReferralRequest_GeneratesExpectedFileData()
    {
        // Arrange
        var underTest = new CsvFileCreator(new NullLogger<CsvFileCreator>());
        var requestTime1 = DateTime.Now;
        var requestDate1 = new DateTime(requestTime1.Year, requestTime1.Month, 5);
        var referralRequest1 = new ReferralRequestBuilder(1)
            .WithCustodianCode("114")
            .WithRequestDate(requestDate1)
            .Build();
        var requestTime2 = DateTime.Now.AddMonths(-3);
        var requestDate2 = new DateTime(requestTime2.Year, requestTime2.Month, 5);
        var referralRequest2 = new ReferralRequestBuilder(6)
            .WithCustodianCode("235")
            .WithRequestDate(requestDate2)
            .Build();
        var requestTime3 = DateTime.Now.AddMonths(-3);
        var requestDate3 = new DateTime(requestTime3.Year, requestTime3.Month, 5);
        var referralRequest3 = new ReferralRequestBuilder(3)
            .WithCustodianCode("121")
            .WithRequestDate(requestDate3)
            .Build();
        var requestTime4 = DateTime.Now.AddMonths(-3);
        var requestDate4 = new DateTime(requestTime4.Year, requestTime4.Month, 5);
        var referralRequest4 = new ReferralRequestBuilder(3)
            .WithCustodianCode("121")
            .WithRequestDate(requestDate4)
            .Build();

        var referralRequests = new List<ReferralRequest>
            { referralRequest1, referralRequest2, referralRequest3, referralRequest4 };
        var consortium1 = GetLocalAuthorityDetails(referralRequest1).Consortium;
        var consortium2 = GetLocalAuthorityDetails(referralRequest2).Consortium;

        // Act
        var data = underTest.CreatePerMonthConsortiumReferralStatisticsForConsole(referralRequests);

        // Assert
        var reader = new StreamReader(data, Encoding.UTF8);
        reader.ReadToEnd().Should().Be(
            "Consortium Name,Total WH:LG Referrals,Date of First Referral,Months Since First Referral,Referrals Per Month\r\n" +
            $"{consortium1},3,{requestDate3:dd/MM/yyyy},3,1\r\n" +
            $"{consortium2},1,{requestDate2:dd/MM/yyyy},3,0.33\r\n");
    }

    [Test]
    public void
        CreatePerMonthConsortiumReferralStatisticsForConsole_CalledWithNonConsortiumReferralRequest_GeneratesFileDataWithoutNonConsortiumRequests()
    {
        // Arrange
        var underTest = new CsvFileCreator(new NullLogger<CsvFileCreator>());
        var requestTime1 = DateTime.Now.AddMonths(-3);
        var referralRequest1 = new ReferralRequestBuilder(3)
            .WithCustodianCode("440")
            .WithRequestDate(new DateTime(requestTime1.Year, requestTime1.Month, 5))
            .Build();

        var referralRequests = new List<ReferralRequest>
            { referralRequest1 };

        // Act
        var data = underTest.CreatePerMonthConsortiumReferralStatisticsForConsole(referralRequests);

        // Assert
        var reader = new StreamReader(data, Encoding.UTF8);
        reader.ReadToEnd().Should().Be(
            "Consortium Name,Total WH:LG Referrals,Date of First Referral,Months Since First Referral,Referrals Per Month\r\n");
    }

    [Test]
    public void
        CreatePerMonthConsortiumReferralStatisticsForConsole_CalledWithReferralRequest_DoesNotIncludeBOMInTheMemoryStream()
    {
        // Arrange
        var underTest = new CsvFileCreator(new NullLogger<CsvFileCreator>());
        var referralRequest = new ReferralRequestBuilder(1).Build();
        var referralRequests = new List<ReferralRequest> { referralRequest };

        // Act
        var data = underTest.CreatePerMonthConsortiumReferralStatisticsForConsole(referralRequests);

        // Assert
        ContainsBom(data).Should().BeFalse();
    }

    private static void VerifySkippingRowLoggedOnce(Mock<ILogger<CsvFileCreator>> mockLogger,
        string expectedSubstringInMessage)
    {
        var errorLogs = mockLogger.Invocations
            .Where(i => i.Method.Name == nameof(ILogger.Log) && (LogLevel)i.Arguments[0] == LogLevel.Error)
            .Select(i => i.Arguments[2]?.ToString() ?? "")
            .Where(msg => msg.Contains("[CsvFileCreator] Skipping row (", StringComparison.Ordinal)
                          && msg.Contains("due to exception.", StringComparison.Ordinal)
                          && msg.Contains(expectedSubstringInMessage, StringComparison.Ordinal))
            .ToList();

        errorLogs.Should().ContainSingle();
    }

    private static LocalAuthorityData.LocalAuthorityDetails GetLocalAuthorityDetails(ReferralRequest referralRequest)
    {
        return LocalAuthorityData.LocalAuthorityDetailsByCustodianCode[referralRequest.CustodianCode];
    }

    private static bool ContainsBom(MemoryStream stream)
    {
        var utf8NoBom = new UTF8Encoding(false);
        using var reader = new StreamReader(stream, utf8NoBom);
        // if there is a BOM in the stream, the encoding will change to a BOM including encoding when its read 
        reader.Read();
        // this will make it no longer equal to this encoding where BOM is set to false
        return !Equals(reader.CurrentEncoding, utf8NoBom);
    }
}