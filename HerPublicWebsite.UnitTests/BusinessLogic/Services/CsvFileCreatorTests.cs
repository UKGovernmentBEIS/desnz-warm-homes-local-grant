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
    public void CreateFileData_CalledWithReferralRequest_GeneratesExpectedFileData()
    {
        // Arrange
        var underTest = new CsvFileCreator();
        var referralRequest = new ReferralRequestBuilder(1).Build();
        var referralRequests = new List<ReferralRequest>() { referralRequest };

        // Act
        var data = underTest.CreateFileData(referralRequests);
        
        // Assert
        var reader = new StreamReader(data, Encoding.UTF8);
        reader.ReadToEnd().Should().Be(
"Referral date,Name,Email,Telephone,Address1,Address2,Town,County,Postcode,UPRN,EPC Band,Is off gas grid,Household income band,Is eligible postcode,Tenure\r\n" +
"2023-01-01 13:00:01,Full Name1,contact1@example.com,00001 123456,Address 1 line 1,Address 1 line 2,Town1,County1,AL01 1RS,100 111 222 001,E,yes,Below £31k,no,Owner\r\n");
    }
    
    [Test]
    public void CreateFileData_CalledWithReferralRequestWithGasBoiler_GeneratesExpectedFileData()
    {
        // Arrange
        var underTest = new CsvFileCreator();
        var referralRequest = new ReferralRequestBuilder(1).WithHasGasBoiler(HasGasBoiler.Yes).Build();
        var referralRequests = new List<ReferralRequest>() { referralRequest };

        // Act
        var data = underTest.CreateFileData(referralRequests);
        
        // Assert
        var reader = new StreamReader(data, Encoding.UTF8);
        reader.ReadToEnd().Should().Be(
"Referral date,Name,Email,Telephone,Address1,Address2,Town,County,Postcode,UPRN,EPC Band,Is off gas grid,Household income band,Is eligible postcode,Tenure\r\n" +
"2023-01-01 13:00:01,Full Name1,contact1@example.com,00001 123456,Address 1 line 1,Address 1 line 2,Town1,County1,AL01 1RS,100 111 222 001,E,no,Below £31k,no,Owner\r\n");
    }
    
    [Test]
    public void CreateFileData_CalledWithReferralRequestWithIncomeAbove31k_GeneratesExpectedFileData()
    {
        // Arrange
        var underTest = new CsvFileCreator();
        var referralRequest = new ReferralRequestBuilder(1).WithIncomeBand(IncomeBand.GreaterThan31000).Build();
        var referralRequests = new List<ReferralRequest>() { referralRequest };

        // Act
        var data = underTest.CreateFileData(referralRequests);
        
        // Assert
        var reader = new StreamReader(data, Encoding.UTF8);
        reader.ReadToEnd().Should().Be(
"Referral date,Name,Email,Telephone,Address1,Address2,Town,County,Postcode,UPRN,EPC Band,Is off gas grid,Household income band,Is eligible postcode,Tenure\r\n" +
"2023-01-01 13:00:01,Full Name1,contact1@example.com,00001 123456,Address 1 line 1,Address 1 line 2,Town1,County1,AL01 1RS,100 111 222 001,E,yes,£31k or above,no,Owner\r\n");
    }
    
    [Test]
    public void CreateFileData_CalledWithMultipleReferralRequests_GeneratesExpectedFileData()
    {
        // Arrange
        var underTest = new CsvFileCreator();
        var referralRequest1 = new ReferralRequestBuilder(1).Build();
        var referralRequest2 = new ReferralRequestBuilder(2).Build();
        var referralRequest3 = new ReferralRequestBuilder(3).Build();
        var referralRequests = new List<ReferralRequest>() { referralRequest1, referralRequest2, referralRequest3 };

        // Act
        var data = underTest.CreateFileData(referralRequests);
        
        // Assert
        var reader = new StreamReader(data, Encoding.UTF8);
        reader.ReadToEnd().Should().Be(
"Referral date,Name,Email,Telephone,Address1,Address2,Town,County,Postcode,UPRN,EPC Band,Is off gas grid,Household income band,Is eligible postcode,Tenure\r\n" +
"2023-01-01 13:00:01,Full Name1,contact1@example.com,00001 123456,Address 1 line 1,Address 1 line 2,Town1,County1,AL01 1RS,100 111 222 001,E,yes,Below £31k,no,Owner\r\n" +
"2023-01-01 13:00:02,Full Name2,contact2@example.com,00002 123456,Address 2 line 1,Address 2 line 2,Town2,County2,AL02 1RS,100 111 222 002,E,yes,Below £31k,no,Owner\r\n" +
"2023-01-01 13:00:03,Full Name3,contact3@example.com,00003 123456,Address 3 line 1,Address 3 line 2,Town3,County3,AL03 1RS,100 111 222 003,E,yes,Below £31k,no,Owner\r\n");
    }

    [Test]
    public void CreateFileData_CalledWithSpecialCharacters_GeneratesEscapedFileData()
    {
        // Arrange
        var underTest = new CsvFileCreator();
        var referralRequest1 = new ReferralRequestBuilder(1).WithFullName("comma,separated,value").Build();
        var referralRequest2 = new ReferralRequestBuilder(2).WithFullName("double\"quotes").Build();
        var referralRequest3 = new ReferralRequestBuilder(3).WithFullName("=Formula()").Build();
        var referralRequest4 = new ReferralRequestBuilder(4).WithTelephone("+441234567890").Build();
        var referralRequests = new List<ReferralRequest>() { referralRequest1, referralRequest2, referralRequest3, referralRequest4 };

        // Act
        var data = underTest.CreateFileData(referralRequests);

        // Assert
        var reader = new StreamReader(data, Encoding.UTF8);
        reader.ReadToEnd().Should().Be(
            "Referral date,Name,Email,Telephone,Address1,Address2,Town,County,Postcode,UPRN,EPC Band,Is off gas grid,Household income band,Is eligible postcode,Tenure\r\n" +
            "2023-01-01 13:00:01,\"comma,separated,value\",contact1@example.com,00001 123456,Address 1 line 1,Address 1 line 2,Town1,County1,AL01 1RS,100 111 222 001,E,yes,Below £31k,no,Owner\r\n" +
            "2023-01-01 13:00:02,\"double\"\"quotes\",contact2@example.com,00002 123456,Address 2 line 1,Address 2 line 2,Town2,County2,AL02 1RS,100 111 222 002,E,yes,Below £31k,no,Owner\r\n" +
            "2023-01-01 13:00:03,Formula(),contact3@example.com,00003 123456,Address 3 line 1,Address 3 line 2,Town3,County3,AL03 1RS,100 111 222 003,E,yes,Below £31k,no,Owner\r\n" +
            "2023-01-01 13:00:04,Full Name4,contact4@example.com,441234567890,Address 4 line 1,Address 4 line 2,Town4,County4,AL04 1RS,100 111 222 004,E,yes,Below £31k,no,Owner\r\n");
    }
}
