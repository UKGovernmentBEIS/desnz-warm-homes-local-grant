using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using HerPublicWebsite.BusinessLogic;
using HerPublicWebsite.BusinessLogic.ExternalServices.S3FileWriter;
using HerPublicWebsite.BusinessLogic.Models;
using HerPublicWebsite.BusinessLogic.Models.Enums;
using NUnit.Framework;
using HerPublicWebsite.BusinessLogic.Services.RegularJobs;
using Moq;

namespace Tests.BusinessLogic.Services;

[TestFixture]
public class RegularJobsServiceTests
{
    private IRegularJobsService regularJobsService;
    private Mock<IDataAccessProvider> mockDataAccessProvider;
    private Mock<IS3FileWriter> mockS3FileWriter;
    
    [SetUp]
    public void Setup()
    {
        mockDataAccessProvider = new Mock<IDataAccessProvider>();
        mockS3FileWriter = new Mock<IS3FileWriter>();
        regularJobsService = new RegularJobsService(mockDataAccessProvider.Object, mockS3FileWriter.Object);
    }

    [Test]
    public async Task RunNightlyTasksAsync_WhenCalledWithNewReferral_UpdatesReferralCreated()
    {
        // Arrange
        var newReferralList = new List<ReferralRequest>
        {
            CreateSubmittedReferralRequest(5, new DateTime(2023, 03, 01)),
            CreateUnsubmittedReferralRequest(5, new DateTime(2023, 03, 02)),
            CreateUnsubmittedReferralRequest(5, new DateTime(2023, 03, 03))
        };
        mockDataAccessProvider.Setup(dap => dap.GetUnsubmittedReferralRequestsAsync().Result).Returns(newReferralList);
        mockDataAccessProvider.Setup(dap => dap.GetReferralRequestsByCustodianAndRequestDateAsync(5, 3, 2023).Result)
            .Returns(newReferralList);
        
        // Act
        await regularJobsService.RunNightlyTasksAsync();

        // Assert
        newReferralList.Should().AllSatisfy(rr => rr.ReferralCreated.Should().BeTrue());
        mockDataAccessProvider.Verify(dap => dap.PersistAllChangesAsync());
    }
    
    private ReferralRequest CreateUnsubmittedReferralRequest(int custodianCode, DateTime requestDate)
    {
        return GetReferralRequest(custodianCode, requestDate, false);
    }
    
    private ReferralRequest CreateSubmittedReferralRequest(int custodianCode, DateTime requestDate)
    {
        return GetReferralRequest(custodianCode, requestDate, true);
    }

    private ReferralRequest GetReferralRequest(int custodianCode, DateTime requestDate, bool hasBeenSubmitted)
    {
        return new ReferralRequest
        {
            AddressLine1 = "addressLine1",
            AddressTown = "town",
            AddressPostcode = "postcode",
            CustodianCode = custodianCode,
            EpcRating = EpcRating.F,
            HasGasBoiler = HasGasBoiler.No,
            IncomeBand = IncomeBand.Under31000,
            IsLsoaProperty = false,
            Uprn = "12345678",
            ReferralCreated = hasBeenSubmitted,
            RequestDate = requestDate,
        };
    }
}