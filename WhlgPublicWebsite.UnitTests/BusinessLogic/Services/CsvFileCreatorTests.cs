using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FluentAssertions;
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
    [Test]
    public void CreateReferralRequestFileData_CalledWithReferralRequest_GeneratesExpectedFileData()
    {
        // Arrange
        var underTest = new CsvFileCreator();
        var referralRequest = new ReferralRequestBuilder(1).Build();
        var referralRequests = new List<ReferralRequest> { referralRequest };

        // Act
        var data = underTest.CreateReferralRequestFileData(referralRequests);

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
    public void CreateReferralRequestFileData_CalledWithReferralRequestWithIncomeAbove31k_GeneratesExpectedFileData(
        IncomeBand incomeBand, string expectedValue)
    {
        // Arrange
        var underTest = new CsvFileCreator();
        var referralRequest = new ReferralRequestBuilder(1).WithIncomeBand(incomeBand).Build();
        var referralRequests = new List<ReferralRequest> { referralRequest };

        // Act
        var data = underTest.CreateReferralRequestFileData(referralRequests);

        // Assert
        var reader = new StreamReader(data, Encoding.UTF8);
        reader.ReadToEnd().Should().Be(
            "Referral date,Referral code,Name,Email,Telephone,Address1,Address2,Town,County,Postcode,UPRN,EPC Band,EPC confirmed by homeowner,EPC Lodgement Date,Household income band,Is eligible postcode,Tenure\r\n" +
            $"2023-01-01 13:00:01,DummyCode00001,Full Name1,contact1@example.com,00001 123456,Address 1 line 1,Address 1 line 2,Town1,County1,AL01 1RS,100 111 222 001,E,,2023-01-01 15:00:01,{expectedValue},no,Owner\r\n");
    }

    [Test]
    public void CreateReferralRequestFileData_CalledWithReferralRequestWithUnsureEpc_GeneratesExpectedFileData()
    {
        // Arrange
        var underTest = new CsvFileCreator();
        var referralRequest = new ReferralRequestBuilder(1).WithEpcConfirmation(EpcConfirmation.Unknown).Build();
        var referralRequests = new List<ReferralRequest> { referralRequest };

        // Act
        var data = underTest.CreateReferralRequestFileData(referralRequests);

        // Assert
        var reader = new StreamReader(data, Encoding.UTF8);
        reader.ReadToEnd().Should().Be(
            "Referral date,Referral code,Name,Email,Telephone,Address1,Address2,Town,County,Postcode,UPRN,EPC Band,EPC confirmed by homeowner,EPC Lodgement Date,Household income band,Is eligible postcode,Tenure\r\n" +
            "2023-01-01 13:00:01,DummyCode00001,Full Name1,contact1@example.com,00001 123456,Address 1 line 1,Address 1 line 2,Town1,County1,AL01 1RS,100 111 222 001,E,Homeowner unsure,2023-01-01 15:00:01,\"£36,000 or less\",no,Owner\r\n");
    }

    [Test]
    public void CreateReferralRequestFileData_CalledWithMultipleReferralRequests_GeneratesExpectedFileData()
    {
        // Arrange
        var underTest = new CsvFileCreator();
        var referralRequest1 = new ReferralRequestBuilder(1).Build();
        var referralRequest2 = new ReferralRequestBuilder(2).Build();
        var referralRequest3 = new ReferralRequestBuilder(3).Build();
        var referralRequests = new List<ReferralRequest> { referralRequest1, referralRequest2, referralRequest3 };

        // Act
        var data = underTest.CreateReferralRequestFileData(referralRequests);

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
    public void CreateReferralRequestFileData_CalledWithSpecialCharacters_GeneratesEscapedFileData(string nameInput,
        string expectedOutput)
    {
        // Arrange
        var underTest = new CsvFileCreator();
        var referralRequest = new ReferralRequestBuilder(1).WithFullName(nameInput).Build();
        var referralRequests = new List<ReferralRequest> { referralRequest };

        // Act
        var data = underTest.CreateReferralRequestFileData(referralRequests);

        // Assert
        var reader = new StreamReader(data, Encoding.UTF8);
        reader.ReadToEnd().Should().Be(
            "Referral date,Referral code,Name,Email,Telephone,Address1,Address2,Town,County,Postcode,UPRN,EPC Band,EPC confirmed by homeowner,EPC Lodgement Date,Household income band,Is eligible postcode,Tenure\r\n" +
            $"2023-01-01 13:00:01,DummyCode00001,{expectedOutput},contact1@example.com,00001 123456,Address 1 line 1,Address 1 line 2,Town1,County1,AL01 1RS,100 111 222 001,E,,2023-01-01 15:00:01,\"£36,000 or less\",no,Owner\r\n");
    }

    [Test]
    public void CreateReferralRequestOverviewFileData_CalledWithReferralRequest_GeneratesExpectedFileData()
    {
        // Arrange
        var underTest = new CsvFileCreator();
        var referralRequest = new ReferralRequestBuilder(1).WithCustodianCode("114").Build();
        var referralRequests = new List<ReferralRequest> { referralRequest };

        // Act
        var data = underTest.CreateReferralRequestOverviewFileData(referralRequests);

        // Assert
        var reader = new StreamReader(data, Encoding.UTF8);
        reader.ReadToEnd().Should().Be(
            "Consortium,Local Authority,Referral Code\r\n" +
            "Bristol City Council,Bath and North East Somerset Council,DummyCode00001\r\n");
    }

    [Test]
    public void
        CreateReferralRequestOverviewFileData_CalledWithReferralRequestFromNullConsortium_GeneratesExpectedFileData()
    {
        // Arrange
        var underTest = new CsvFileCreator();
        var referralRequest = new ReferralRequestBuilder(1).WithCustodianCode("9052")
            .WithFollowUp(new ReferralRequestFollowUpBuilder(1)).Build();
        var referralRequests = new List<ReferralRequest> { referralRequest };

        // Act
        var data = underTest.CreateReferralRequestOverviewFileData(referralRequests);

        // Assert
        var reader = new StreamReader(data, Encoding.UTF8);
        reader.ReadToEnd().Should().Be(
            "Consortium,Local Authority,Referral Code\r\n" +
            ",Aberdeenshire Council,DummyCode00001\r\n");
    }

    [Test]
    public void CreateLocalAuthorityReferralRequestFollowUpData_CalledWithReferralRequest_GeneratesExpectedFileData()
    {
        // Arrange
        var underTest = new CsvFileCreator();
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
        var today = DateTime.Today.ToString("dd-MMM");

        // Act
        var data = underTest.CreateLocalAuthorityReferralRequestFollowUpFileData(referralRequests);

        // Assert
        var reader = new StreamReader(data, Encoding.UTF8);
        reader.ReadToEnd().Should().Be(
            "SLA Report Date,LA,LA Number of Referrals Not Downloaded,LA Percentage of Referrals Not Downloaded," +
            "LA Number of Referrals Not Contacted,LA Percentage of Referrals Not Contacted,LA Number of Referrals Responded to email,LA Percentage of Referrals Responded to email\r\n" +
            $"{today},Bath and North East Somerset Council,2,100,1,50,1,50\r\n" + // Custodian Code 114
            $"{today},North Somerset Council,1,33.333333333333336,1,33.333333333333336,3,100\r\n" + // Custodian Code 121
            $"{today},Aberdeenshire Council,1,33.333333333333336,2,66.66666666666667,3,100\r\n" // Custodian Code 9052
        );
    }

    [Test]
    public void CreateConsortiumReferralRequestFollowUpData_CalledWithReferralRequest_GeneratesExpectedFileData()
    {
        // Arrange
        var underTest = new CsvFileCreator();
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

        var today = DateTime.Today.ToString("dd-MMM");
        // Act
        var data = underTest.CreateConsortiumReferralRequestFollowUpFileData(referralRequests);

        // Assert
        var reader = new StreamReader(data, Encoding.UTF8);
        reader.ReadToEnd().Should().Be(
            "SLA Report Date,Consortium,Consortium All Referrals Downloaded,Consortium Number of Referrals Not Downloaded," +
            "Consortium Percentage of Referrals Not Downloaded,Consortium All Referrals Contacted," +
            "Consortium Number of Referrals Not Contacted,Consortium Percentage of Referrals Not Contacted\r\n" +
            $"{today},Bristol City Council,False,3,60,False,2,40\r\n" // Custodian codes 114 and 121"
        ); // Stats for 9052 and 2205 with no Consortium name should not appear as LAs with no Consortium should not be included
    }

    [Test]
    public void CreatePendingReferralRequestFileData_CalledWithReferralRequest_GeneratesExpectedFileData()
    {
        // Arrange
        var underTest = new CsvFileCreator();
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

        var referralRequests = new List<ReferralRequest> { referralRequest1, referralRequest2, referralRequest3 };

        // Act
        var data = underTest.CreatePendingReferralRequestFileData(referralRequests);

        // Assert
        var reader = new StreamReader(data, Encoding.UTF8);
        reader.ReadToEnd().Should().Be(
            "Consortium,Local Authority,Referral Date,Referral Code,Name,Email,Telephone,Local Authority Status\r\n" +
            "Bristol City Council,Bath and North East Somerset Council,2024-03-05 01:00:00,TEST0001,Test User 1,test1@example.com,111,Pending\r\n" +
            "Portsmouth City Council,Bedford Borough Council,2024-02-05 01:00:00,TEST0002,Test User 2,test2@example.com,,Pending\r\n" +
            "Bristol City Council,North Somerset Council,2024-01-05 01:00:00,TEST0003,Test User 3,,333,Pending\r\n");
    }

    [Test]
    public void CreatePerMonthLocalAuthorityReferralStatistics_CalledWithReferralRequest_GeneratesExpectedFileData()
    {
        // Arrange
        var underTest = new CsvFileCreator();
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

        // Act
        var data = underTest.CreatePerMonthLocalAuthorityReferralStatistics(referralRequests);

        // Assert
        var reader = new StreamReader(data, Encoding.UTF8);
        reader.ReadToEnd().Should().Be(
            "LA Name,Consortium Name,Total WH:LG Referrals,Date of First Referral,Months Since First Referral,Referrals Per Month\r\n" +
            $"Bath and North East Somerset Council,Bristol City Council,1,{requestDate1:dd/MM/yyyy},1,1\r\n" +
            $"Bedford Borough Council,Portsmouth City Council,1,{requestDate2:dd/MM/yyyy},3,0.33\r\n" +
            $"North Somerset Council,Bristol City Council,3,{requestDate3:dd/MM/yyyy},3,1\r\n");
    }

    [Test]
    public void CreatePerMonthConsortiumReferralStatistics_CalledWithReferralRequest_GeneratesExpectedFileData()
    {
        // Arrange
        var underTest = new CsvFileCreator();
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

        // Act
        var data = underTest.CreatePerMonthConsortiumReferralStatistics(referralRequests);

        // Assert
        var reader = new StreamReader(data, Encoding.UTF8);
        reader.ReadToEnd().Should().Be(
            "Consortium Name,Total WH:LG Referrals,Date of First Referral,Months Since First Referral,Referrals Per Month\r\n" +
            $"Bristol City Council,3,{requestDate2:dd/MM/yyyy},3,1\r\n" +
            $"Portsmouth City Council,1,{requestDate3:dd/MM/yyyy},3,0.33\r\n");
    }
    
        [Test]
    public void CreatePerMonthConsortiumReferralStatistics_CalledWithNonConsortiumReferralRequest_GeneratesFileDataWithoutNonConsortiumRequests()
    {
        // Arrange
        var underTest = new CsvFileCreator();
        var requestTime1 = DateTime.Now.AddMonths(-3);
        var referralRequest1 = new ReferralRequestBuilder(3)
            .WithCustodianCode("440")
            .WithRequestDate(new DateTime(requestTime1.Year, requestTime1.Month, 5))
            .Build();

        var referralRequests = new List<ReferralRequest>
            { referralRequest1 };

        // Act
        var data = underTest.CreatePerMonthConsortiumReferralStatistics(referralRequests);

        // Assert
        var reader = new StreamReader(data, Encoding.UTF8);
        reader.ReadToEnd().Should().Be(
            "Consortium Name,Total WH:LG Referrals,Date of First Referral,Months Since First Referral,Referrals Per Month\r\n");
    }
}