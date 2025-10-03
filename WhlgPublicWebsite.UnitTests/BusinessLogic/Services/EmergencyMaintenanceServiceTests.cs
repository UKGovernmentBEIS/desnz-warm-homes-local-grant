using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using WhlgPublicWebsite.BusinessLogic;
using WhlgPublicWebsite.BusinessLogic.Models;
using WhlgPublicWebsite.BusinessLogic.Services.EmergencyMaintenance;

namespace Tests.BusinessLogic.Services;

public class EmergencyMaintenanceServiceTests
{
    private Mock<IDataAccessProvider> mockDataAccessProvider;
    private EmergencyMaintenanceService underTest;

    [SetUp]
    public void Setup()
    {
        mockDataAccessProvider = new Mock<IDataAccessProvider>();

        underTest = new EmergencyMaintenanceService(mockDataAccessProvider.Object);
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