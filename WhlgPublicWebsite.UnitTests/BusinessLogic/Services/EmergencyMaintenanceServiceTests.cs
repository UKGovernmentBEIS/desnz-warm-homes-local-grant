using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using WhlgPublicWebsite.BusinessLogic;
using WhlgPublicWebsite.BusinessLogic.Models;
using WhlgPublicWebsite.BusinessLogic.Services.EmergencyMaintenance;

namespace Tests.BusinessLogic.Services;

public class EmergencyMaintenanceServiceTests
{
    private Mock<IServiceScopeFactory> mockScopeFactory;
    private Mock<IServiceScope> mockScope;
    private Mock<IServiceProvider> mockServiceProvider;
    private Mock<IDataAccessProvider> mockDataAccessProvider;
    private EmergencyMaintenanceService underTest;

    [SetUp]
    public void Setup()
    {
        mockScopeFactory = new Mock<IServiceScopeFactory>();
        mockScope = new Mock<IServiceScope>();
        mockServiceProvider = new Mock<IServiceProvider>();
        mockDataAccessProvider = new Mock<IDataAccessProvider>();
 
        mockScopeFactory.Setup(x => x.CreateScope()).Returns(mockScope.Object);
    
        mockScope.Setup(x => x.ServiceProvider).Returns(mockServiceProvider.Object);
    
        mockServiceProvider.Setup(x => x.GetService(typeof(IDataAccessProvider)))
            .Returns(mockDataAccessProvider.Object);
 
        underTest = new EmergencyMaintenanceService(mockScopeFactory.Object);;
    }

    [TestCase(EmergencyMaintenanceState.Enabled, true)]
    [TestCase(EmergencyMaintenanceState.Disabled, false)]
    [TestCase(null, false)]
    public async Task SiteIsInEmergencyMaintenance_WhenCalled_ReturnsExpectedState(EmergencyMaintenanceState? state,
        bool expected)
    {
        // Arrange
        mockDataAccessProvider
            .Setup(dap => dap.GetLatestEmergencyMaintenanceHistoryAsync())
            .ReturnsAsync(state == null ? null : new EmergencyMaintenanceHistory { State = state.Value });

        // Act
        var result = await underTest.SiteIsInEmergencyMaintenance();

        // Assert
        result.Should().Be(expected);
    }
}