using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FluentAssertions;
using HerPublicWebsite.BusinessLogic.Models;
using HerPublicWebsite.BusinessLogic.Models.Enums;
using HerPublicWebsite.BusinessLogic.Services.CsvFileCreator;
using NUnit.Framework;
using Tests.Builders;

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
        var referralRequests = new List<ReferralRequest>() { referralRequest };

        // Act
        var data = underTest.CreateReferralRequestFileData(referralRequests);
        
        // Assert
        var reader = new StreamReader(data, Encoding.UTF8);
        reader.ReadToEnd().Should().Be(
"Referral date,Referral code,Name,Email,Telephone,Address1,Address2,Town,County,Postcode,UPRN,EPC Band,EPC confirmed by homeowner,EPC Lodgement Date,Is off gas grid,Household income band,Is eligible postcode,Tenure\r\n" +
"2023-01-01 13:00:01,DummyCode00001,Full Name1,contact1@example.com,00001 123456,Address 1 line 1,Address 1 line 2,Town1,County1,AL01 1RS,100 111 222 001,E,,2023-01-01 15:00:01,yes,£36k or below,no,Owner\r\n");
    }
    
    [Test]
    public void CreateReferralRequestFileData_CalledWithReferralRequestWithGasBoiler_GeneratesExpectedFileData()
    {
        // Arrange
        var underTest = new CsvFileCreator();
        var referralRequest = new ReferralRequestBuilder(1).WithHasGasBoiler(HasGasBoiler.Yes).Build();
        var referralRequests = new List<ReferralRequest>() { referralRequest };

        // Act
        var data = underTest.CreateReferralRequestFileData(referralRequests);
        
        // Assert
        var reader = new StreamReader(data, Encoding.UTF8);
        reader.ReadToEnd().Should().Be(
"Referral date,Referral code,Name,Email,Telephone,Address1,Address2,Town,County,Postcode,UPRN,EPC Band,EPC confirmed by homeowner,EPC Lodgement Date,Is off gas grid,Household income band,Is eligible postcode,Tenure\r\n" +
"2023-01-01 13:00:01,DummyCode00001,Full Name1,contact1@example.com,00001 123456,Address 1 line 1,Address 1 line 2,Town1,County1,AL01 1RS,100 111 222 001,E,,2023-01-01 15:00:01,no,£36k or below,no,Owner\r\n");
    }
    
    [TestCase(IncomeBand.GreaterThan31000, "£31k or above")]
    [TestCase(IncomeBand.GreaterThan34500, "£34.5k or above")]
    [TestCase(IncomeBand.GreaterThan36000, "Above £36k")]
    [TestCase(IncomeBand.UnderOrEqualTo31000, "Below £31k")]
    [TestCase(IncomeBand.UnderOrEqualTo34500, "Below £34.5k")]
    [TestCase(IncomeBand.UnderOrEqualTo36000, "£36k or below")]
    
    public void CreateReferralRequestFileData_CalledWithReferralRequestWithIncomeAbove31k_GeneratesExpectedFileData(IncomeBand incomeBand, string expectedValue)
    {
        // Arrange
        var underTest = new CsvFileCreator();
        var referralRequest = new ReferralRequestBuilder(1).WithIncomeBand(incomeBand).Build();
        var referralRequests = new List<ReferralRequest>() { referralRequest };

        // Act
        var data = underTest.CreateReferralRequestFileData(referralRequests);
        
        // Assert
        var reader = new StreamReader(data, Encoding.UTF8);
        reader.ReadToEnd().Should().Be(
"Referral date,Referral code,Name,Email,Telephone,Address1,Address2,Town,County,Postcode,UPRN,EPC Band,EPC confirmed by homeowner,EPC Lodgement Date,Is off gas grid,Household income band,Is eligible postcode,Tenure\r\n" +
$"2023-01-01 13:00:01,DummyCode00001,Full Name1,contact1@example.com,00001 123456,Address 1 line 1,Address 1 line 2,Town1,County1,AL01 1RS,100 111 222 001,E,,2023-01-01 15:00:01,yes,{expectedValue},no,Owner\r\n");
    }

    [Test]
    public void CreateReferralRequestFileData_CalledWithReferralRequestWithUnsureEpc_GeneratesExpectedFileData()
    {
        // Arrange
        var underTest = new CsvFileCreator();
        var referralRequest = new ReferralRequestBuilder(1).WithEpcConfirmation(EpcConfirmation.Unknown).Build();
        var referralRequests = new List<ReferralRequest>() { referralRequest };

        // Act
        var data = underTest.CreateReferralRequestFileData(referralRequests);
        
        // Assert
        var reader = new StreamReader(data, Encoding.UTF8);
        reader.ReadToEnd().Should().Be(
"Referral date,Referral code,Name,Email,Telephone,Address1,Address2,Town,County,Postcode,UPRN,EPC Band,EPC confirmed by homeowner,EPC Lodgement Date,Is off gas grid,Household income band,Is eligible postcode,Tenure\r\n" +
"2023-01-01 13:00:01,DummyCode00001,Full Name1,contact1@example.com,00001 123456,Address 1 line 1,Address 1 line 2,Town1,County1,AL01 1RS,100 111 222 001,E,Homeowner unsure,2023-01-01 15:00:01,yes,£36k or below,no,Owner\r\n");
    }
    
    [Test]
    public void CreateReferralRequestFileData_CalledWithMultipleReferralRequests_GeneratesExpectedFileData()
    {
        // Arrange
        var underTest = new CsvFileCreator();
        var referralRequest1 = new ReferralRequestBuilder(1).Build();
        var referralRequest2 = new ReferralRequestBuilder(2).Build();
        var referralRequest3 = new ReferralRequestBuilder(3).Build();
        var referralRequests = new List<ReferralRequest>() { referralRequest1, referralRequest2, referralRequest3 };

        // Act
        var data = underTest.CreateReferralRequestFileData(referralRequests);
        
        // Assert
        var reader = new StreamReader(data, Encoding.UTF8);
        reader.ReadToEnd().Should().Be(
"Referral date,Referral code,Name,Email,Telephone,Address1,Address2,Town,County,Postcode,UPRN,EPC Band,EPC confirmed by homeowner,EPC Lodgement Date,Is off gas grid,Household income band,Is eligible postcode,Tenure\r\n" +
"2023-01-01 13:00:01,DummyCode00001,Full Name1,contact1@example.com,00001 123456,Address 1 line 1,Address 1 line 2,Town1,County1,AL01 1RS,100 111 222 001,E,,2023-01-01 15:00:01,yes,£36k or below,no,Owner\r\n" +
"2023-01-01 13:00:02,DummyCode00002,Full Name2,contact2@example.com,00002 123456,Address 2 line 1,Address 2 line 2,Town2,County2,AL02 1RS,100 111 222 002,E,,2023-01-01 15:00:02,yes,£36k or below,no,Owner\r\n" +
"2023-01-01 13:00:03,DummyCode00003,Full Name3,contact3@example.com,00003 123456,Address 3 line 1,Address 3 line 2,Town3,County3,AL03 1RS,100 111 222 003,E,,2023-01-01 15:00:03,yes,£36k or below,no,Owner\r\n");
    }

    [TestCase("comma,separated,value", "\"comma,separated,value\"")]
    [TestCase("double\"quotes", "\"double\"\"quotes\"")]
    [TestCase("=Formula()", "Formula()")]
    [TestCase("+441234567890", "441234567890")]
    public void CreateReferralRequestFileData_CalledWithSpecialCharacters_GeneratesEscapedFileData(string nameInput, string expectedOutput)
    {
        // Arrange
        var underTest = new CsvFileCreator();
        var referralRequest = new ReferralRequestBuilder(1).WithFullName(nameInput).Build();
        var referralRequests = new List<ReferralRequest>() { referralRequest };

        // Act
        var data = underTest.CreateReferralRequestFileData(referralRequests);

        // Assert
        var reader = new StreamReader(data, Encoding.UTF8);
        reader.ReadToEnd().Should().Be(
"Referral date,Referral code,Name,Email,Telephone,Address1,Address2,Town,County,Postcode,UPRN,EPC Band,EPC confirmed by homeowner,EPC Lodgement Date,Is off gas grid,Household income band,Is eligible postcode,Tenure\r\n" +
$"2023-01-01 13:00:01,DummyCode00001,{expectedOutput},contact1@example.com,00001 123456,Address 1 line 1,Address 1 line 2,Town1,County1,AL01 1RS,100 111 222 001,E,,2023-01-01 15:00:01,yes,£36k or below,no,Owner\r\n");
    }

    [Test]
    public void CreateReferralRequestOverviewFileData_CalledWithReferralRequest_GeneratesExpectedFileData()
    {
        // Arrange
        var underTest = new CsvFileCreator();
        var referralRequest = new ReferralRequestBuilder(1).WithCustodianCode("114").Build();
        var referralRequests = new List<ReferralRequest>() { referralRequest };

        // Act
        var data = underTest.CreateReferralRequestOverviewFileData(referralRequests);
        
        // Assert
        var reader = new StreamReader(data, Encoding.UTF8);
        reader.ReadToEnd().Should().Be(
"Consortium,Local Authority,Referral Code\r\n" +
"Bristol,Bath and North East Somerset Council,DummyCode00001\r\n");
    }
    
    [Test]
    public void CreateReferralRequestOverviewFileData_CalledWithReferralRequestFromNullConsortium_GeneratesExpectedFileData()
    {
        // Arrange
        var underTest = new CsvFileCreator();
        var referralRequest = new ReferralRequestBuilder(1).WithCustodianCode("9052").WithFollowUp(new ReferralRequestFollowUpBuilder(1)).Build();
        var referralRequests = new List<ReferralRequest>() { referralRequest };

        // Act
        var data = underTest.CreateReferralRequestOverviewFileData(referralRequests);
        
        // Assert
        var reader = new StreamReader(data, Encoding.UTF8);
        reader.ReadToEnd().Should().Be(
            "Consortium,Local Authority,Referral Code\r\n" +
            ",Aberdeenshire Council,DummyCode00001\r\n");
    }
    
    [Test]
    public void CreateReferralRequestFollowUpData_CalledWithReferralRequest_GeneratesExpectedFileData()
    {
        // Arrange
        var underTest = new CsvFileCreator();
        var referralRequest1 = new ReferralRequestBuilder(1).WithWrittenToCsv(false).WithCustodianCode("114").WithFollowUp(new ReferralRequestFollowUpBuilder(1).WithWasFollowedUp(null)).Build();
        var referralRequest2 = new ReferralRequestBuilder(2).WithWrittenToCsv(false).WithCustodianCode("114").WithFollowUp(new ReferralRequestFollowUpBuilder(2).WithWasFollowedUp(false)).Build();
        var referralRequest3 = new ReferralRequestBuilder(3).WithWrittenToCsv(true).WithCustodianCode("121").WithFollowUp(new ReferralRequestFollowUpBuilder(3).WithWasFollowedUp(true)).Build();
        var referralRequest4 = new ReferralRequestBuilder(4).WithWrittenToCsv(true).WithCustodianCode("121").WithFollowUp(new ReferralRequestFollowUpBuilder(4).WithWasFollowedUp(false)).Build();
        var referralRequest5 = new ReferralRequestBuilder(5).WithWrittenToCsv(false).WithCustodianCode("121").WithFollowUp(new ReferralRequestFollowUpBuilder(5).WithWasFollowedUp(true)).Build();
        var referralRequest6 = new ReferralRequestBuilder(6).WithWrittenToCsv(false).WithCustodianCode("9052").WithFollowUp(new ReferralRequestFollowUpBuilder(6).WithWasFollowedUp(false)).Build();
        var referralRequest7 = new ReferralRequestBuilder(7).WithWrittenToCsv(true).WithCustodianCode("9052").WithFollowUp(new ReferralRequestFollowUpBuilder(7).WithWasFollowedUp(true)).Build();
        var referralRequest8 = new ReferralRequestBuilder(8).WithWrittenToCsv(true).WithCustodianCode("9052").WithFollowUp(new ReferralRequestFollowUpBuilder(8).WithWasFollowedUp(false)).Build();

        var referralRequests = new List<ReferralRequest>() { referralRequest1, referralRequest2, referralRequest3, referralRequest4, referralRequest5, referralRequest6, referralRequest7, referralRequest8 };

        // Act
        var data = underTest.CreateReferralRequestFollowUpFileData(referralRequests);
        
        // Assert
        var reader = new StreamReader(data, Encoding.UTF8);
        reader.ReadToEnd().Should().Be(
            "Consortium,Consortium All Referrals Downloaded,Consortium Number of Referrals Not Downloaded,"+
            "Consortium Percentage of Referrals Not Downloaded,Consortium All Referrals Contacted,"+
            "Consortium Number of Referrals Not Contacted,Consortium Percentage of Referrals Not Contacted,"+
            "LA,LA Number of Referrals Not Downloaded,LA Percentage of Referrals Not Downloaded,"+
            "LA Number of Referrals Not Contacted,LA Percentage of Referrals Not Contacted,LA Number of Referrals Responded to email,LA Percentage of Referrals Responded to email\r\n"+
            "Bristol,False,3,60,False,2,40,Bath and North East Somerset Council,2,100,1,50,1,50\r\n"+ // Custodian Code 114
            "Bristol,False,3,60,False,2,40,North Somerset Council,1,33.333333333333336,1,33.333333333333336,3,100\r\n"+ // Custodian Code 121
            ",False,1,33.333333333333336,False,2,66.66666666666667,Aberdeenshire Council,1,33.333333333333336,2,66.66666666666667,3,100\r\n" // Custodian Code 9052
            );
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
            "Bristol,Bath and North East Somerset Council,2024-03-05 01:00:00,TEST0001,Test User 1,test1@example.com,111,Live\r\n" +
            "Cambridgeshire & Peterborough Combined Authority,Bedford Borough Council,2024-02-05 01:00:00,TEST0002,Test User 2,test2@example.com,,Pending\r\n" +
            "Bristol,North Somerset Council,2024-01-05 01:00:00,TEST0003,Test User 3,,333,Live\r\n");
    }
}
