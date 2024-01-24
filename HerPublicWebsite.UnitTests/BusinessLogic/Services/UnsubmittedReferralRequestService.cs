using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using HerPublicWebsite.BusinessLogic;
using HerPublicWebsite.BusinessLogic.ExternalServices.S3FileWriter;
using HerPublicWebsite.BusinessLogic.Models;
using HerPublicWebsite.BusinessLogic.Services.CsvFileCreator;
using NUnit.Framework;
using HerPublicWebsite.BusinessLogic.Services.UnsubmittedReferralRequests;
using Moq;
using Tests.Builders;
using RichardSzalay.MockHttp;
using HerPublicWebsite.BusinessLogic.ExternalServices.Common;

namespace Tests.BusinessLogic.Services;

[TestFixture]
public class UnsubmittedReferralRequestsServiceTests
{
    private IUnsubmittedReferralRequestsService unsubmittedReferralRequestsService;
    private Mock<IDataAccessProvider> mockDataAccessProvider;
    private Mock<IS3FileWriter> mockS3FileWriter;
    private MockHttpMessageHandler mockHttpHandler;
    
    [SetUp]
    public void Setup()
    {
        mockDataAccessProvider = new Mock<IDataAccessProvider>();
        mockS3FileWriter = new Mock<IS3FileWriter>();
        unsubmittedReferralRequestsService = new UnsubmittedReferralRequestsService(mockDataAccessProvider.Object, mockS3FileWriter.Object, new CsvFileCreator());
        mockHttpHandler = new MockHttpMessageHandler();
        HttpRequestHelper.handler = mockHttpHandler;
    }

    [Test]
    public async Task WriteUnsubmittedReferralRequestsToCsv_WhenCalledWithNewReferral_UpdatesReferralCreated()
    {
        // Arrange
        var newReferralList = new List<ReferralRequest>
        {
            new ReferralRequestBuilder(1).WithReferralCreated(false).WithRequestDate(new DateTime(2023, 03, 01)).Build()
        };
        mockDataAccessProvider.Setup(dap => dap.GetUnsubmittedReferralRequestsAsync().Result).Returns(newReferralList);
        mockDataAccessProvider.Setup(dap => dap.GetReferralRequestsByCustodianAndRequestDateAsync(newReferralList[0].CustodianCode, 3, 2023).Result)
            .Returns(newReferralList);
        
        // Act
        await unsubmittedReferralRequestsService.WriteUnsubmittedReferralRequestsToCsv();

        // Assert
        newReferralList.Should().AllSatisfy(rr => rr.ReferralWrittenToCsv.Should().BeTrue());
        mockDataAccessProvider.Verify(dap => dap.PersistAllChangesAsync());
    }
    
    [Test]
    public async Task WriteUnsubmittedReferralRequestsToCsv_WhenCalledWithNewReferral_CreatesFile()
    {
        // Arrange
        var newReferralList = new List<ReferralRequest>
        {
            new ReferralRequestBuilder(1).WithReferralCreated(false).WithRequestDate(new DateTime(2023, 03, 01)).Build()
        };
        mockDataAccessProvider.Setup(dap => dap.GetUnsubmittedReferralRequestsAsync().Result).Returns(newReferralList);
        mockDataAccessProvider.Setup(dap => dap.GetReferralRequestsByCustodianAndRequestDateAsync(newReferralList[0].CustodianCode, 3, 2023).Result)
            .Returns(newReferralList);
        
        // Act
        await unsubmittedReferralRequestsService.WriteUnsubmittedReferralRequestsToCsv();

        // Assert
        mockS3FileWriter.Verify(fw =>
            fw.WriteFileAsync(newReferralList[0].CustodianCode, 3, 2023, It.IsAny<MemoryStream>()));
    }
    
    [Test]
    public async Task WriteUnsubmittedReferralRequestsToCsv_WhenCalledWithNewReferralForSameMonthAsOldReferrals_UpdatesReferralCreated()
    {
        // Arrange
        var oldReferral = new ReferralRequestBuilder(1)
            .WithReferralCreated(true)
            .WithRequestDate(new DateTime(2023, 03, 01))
            .Build();
        var newReferral = new ReferralRequestBuilder(1)
            .WithReferralCreated(false)
            .WithRequestDate(new DateTime(2023, 03, 02))
            .Build();
        var newReferralList = new List<ReferralRequest>
        {
            newReferral
        };
        var allReferralList = new List<ReferralRequest>
        {
            oldReferral,
            newReferral
        };
        mockDataAccessProvider.Setup(dap => dap.GetUnsubmittedReferralRequestsAsync().Result).Returns(newReferralList);
        mockDataAccessProvider.Setup(dap => dap.GetReferralRequestsByCustodianAndRequestDateAsync(newReferralList[0].CustodianCode, 3, 2023).Result)
            .Returns(allReferralList);
        
        // Act
        await unsubmittedReferralRequestsService.WriteUnsubmittedReferralRequestsToCsv();

        // Assert
        allReferralList.Should().AllSatisfy(rr => rr.ReferralWrittenToCsv.Should().BeTrue());
        mockDataAccessProvider.Verify(dap => dap.PersistAllChangesAsync());
    }
    
    [Test]
    public async Task WriteUnsubmittedReferralRequestsToCsv_WhenCalledWithMultipleNewReferralsForDifferentCustodianCodes_CreatesMultipleFiles()
    {
        // Arrange
        var newReferral1 = new ReferralRequestBuilder(1)
            .WithReferralCreated(false)
            .WithCustodianCode("5")
            .WithRequestDate(new DateTime(2023, 03, 01))
            .Build();
        var newReferral2 = new ReferralRequestBuilder(1)
            .WithReferralCreated(false)
            .WithCustodianCode("6")
            .WithRequestDate(new DateTime(2023, 03, 02))
            .Build();
        
        var newReferralList = new List<ReferralRequest> { newReferral1, newReferral2 };
        var allReferralListForCustodianCode5 = new List<ReferralRequest> { newReferral1 };
        var allReferralListForCustodianCode6 = new List<ReferralRequest> { newReferral2 };
        mockDataAccessProvider.Setup(dap => dap.GetUnsubmittedReferralRequestsAsync().Result).Returns(newReferralList);
        mockDataAccessProvider.Setup(dap => dap.GetReferralRequestsByCustodianAndRequestDateAsync("5", 3, 2023).Result)
            .Returns(allReferralListForCustodianCode5);
        mockDataAccessProvider.Setup(dap => dap.GetReferralRequestsByCustodianAndRequestDateAsync("6", 3, 2023).Result)
            .Returns(allReferralListForCustodianCode6);
        
        // Act
        await unsubmittedReferralRequestsService.WriteUnsubmittedReferralRequestsToCsv();

        // Assert
        mockS3FileWriter.Verify(fw =>
            fw.WriteFileAsync("5", 3, 2023, It.IsAny<MemoryStream>()));
        mockS3FileWriter.Verify(fw =>
            fw.WriteFileAsync("6", 3, 2023, It.IsAny<MemoryStream>()));
    }
    
   
    [Test]
    public async Task WriteUnsubmittedReferralRequestsToCsv_WhenCalledWritingTheSecondFileFails_UpdatesTheReferralsInTheFirstFileButNotTheSecond()
    {
        // Arrange
        var newReferral1 = new ReferralRequestBuilder(1)
            .WithReferralCreated(false)
            .WithCustodianCode("5")
            .WithRequestDate(new DateTime(2023, 03, 01))
            .Build();
        var newReferral2 = new ReferralRequestBuilder(1)
            .WithReferralCreated(false)
            .WithCustodianCode("6")
            .WithRequestDate(new DateTime(2023, 03, 02))
            .Build();
        
        var newReferralList = new List<ReferralRequest> { newReferral1, newReferral2 };
        var allReferralListForCustodianCode5 = new List<ReferralRequest> { newReferral1 };
        var allReferralListForCustodianCode6 = new List<ReferralRequest> { newReferral2 };
        mockDataAccessProvider.Setup(dap => dap.GetUnsubmittedReferralRequestsAsync().Result).Returns(newReferralList);
        mockDataAccessProvider.Setup(dap => dap.GetReferralRequestsByCustodianAndRequestDateAsync("5", 3, 2023).Result)
            .Returns(allReferralListForCustodianCode5);
        mockDataAccessProvider.Setup(dap => dap.GetReferralRequestsByCustodianAndRequestDateAsync("6", 3, 2023).Result)
            .Returns(allReferralListForCustodianCode6);
        mockS3FileWriter.Setup(fw => fw.WriteFileAsync("6", 3, 2023, It.IsAny<MemoryStream>()))
            .Throws(new InvalidOperationException("Test exception"));
        
        // Act
        try
        {
            await unsubmittedReferralRequestsService.WriteUnsubmittedReferralRequestsToCsv();
        }
        catch (InvalidOperationException e) when (e.Message == "Test exception")
        {
            // This is the exception we deliberately threw during the test.
        }
        

        // Assert
        mockS3FileWriter.Verify(fw =>
            fw.WriteFileAsync("5", 3, 2023, It.IsAny<MemoryStream>()));
        allReferralListForCustodianCode5.Should().AllSatisfy(rr => rr.ReferralWrittenToCsv.Should().BeTrue());
        allReferralListForCustodianCode6.Should().AllSatisfy(rr => rr.ReferralWrittenToCsv.Should().BeFalse());
        mockDataAccessProvider.Verify(dap => dap.PersistAllChangesAsync());
    }
}
