using WhlgPublicWebsite.BusinessLogic.Models;
using WhlgPublicWebsite.BusinessLogic.Services.EmergencyMaintenance;
using WhlgPublicWebsite.Data;

namespace WhlgPublicWebsite.ManagementShell;

public class CommandLineEmergencyMaintenanceService
{
    private EmergencyMaintenanceService EmergencyMaintenanceService { get; }

    public CommandLineEmergencyMaintenanceService(WhlgDbContext context)
    {
        var dataAccessProvider = new DataAccessProvider(context);
        EmergencyMaintenanceService = new EmergencyMaintenanceService(dataAccessProvider);
    }

    public async Task<EmergencyMaintenanceState> GetEmergencyMaintenanceState()
    {
        return await EmergencyMaintenanceService.GetEmergencyMaintenanceState();
    }

    public async Task SetEmergencyMaintenanceState(EmergencyMaintenanceState state, string authorEmail)
    {
        await EmergencyMaintenanceService.SetEmergencyMaintenanceState(state, authorEmail);
    }
}