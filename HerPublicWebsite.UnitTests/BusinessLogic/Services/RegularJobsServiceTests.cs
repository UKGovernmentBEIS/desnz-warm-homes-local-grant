using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using HerPublicWebsite.BusinessLogic;
using HerPublicWebsite.BusinessLogic.Models;
using HerPublicWebsite.BusinessLogic.Models.Enums;
using NUnit.Framework;
using HerPublicWebsite.BusinessLogic.Services.RegularJobs;
using Moq;

namespace Tests.BusinessLogic.Services;

[TestFixture]
public class RegularJobsServiceTests
{
    [SetUp]
    public void Setup()
    {
        mockDataAccessProvider = new Mock<IDataAccessProvider>();
        regularJobsService = new RegularJobsService(mockDataAccessProvider.Object);
    }

    private IRegularJobsService regularJobsService;
    private Mock<IDataAccessProvider> mockDataAccessProvider;

    [Test]
    public async Task RunNightlyTasksAsync_WhenCalledWithNewReferral_UpdatesReferralCreated()
    {
        // Arrange
        var newReferralList = new List<ReferralRequest> { new ReferralRequest
        {
            AddressLine1 = "addressLine1",
            AddressTown = "town",
            AddressPostcode = "postcode",
            CustodianCode = 5,
            EpcRating = EpcRating.F,
            HasGasBoiler = HasGasBoiler.No,
            IncomeBand = IncomeBand.Under31000,
            IsLsoaProperty = false,
            Uprn = "12345678",
            ReferralCreated = false
        }};
        mockDataAccessProvider.Setup(dap => dap.GetUnsubmittedReferralRequestsAsync().Result).Returns(newReferralList);
        
        // Act
        await regularJobsService.RunNightlyTasksAsync();

        // Assert
        newReferralList.Single().ReferralCreated.Should().BeTrue();
        mockDataAccessProvider.Verify(dap => dap.PersistAllChangesAsync());
    }
}