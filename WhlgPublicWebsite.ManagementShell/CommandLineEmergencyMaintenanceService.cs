using WhlgPublicWebsite.BusinessLogic;
using WhlgPublicWebsite.BusinessLogic.Models;
using WhlgPublicWebsite.Data;

namespace WhlgPublicWebsite.ManagementShell;

public class CommandLineEmergencyMaintenanceService(WhlgDbContext context)
{
    private readonly DataAccessProvider dataAccessProvider = new(context);

    public async Task<EmergencyMaintenanceState> GetEmergencyMaintenanceState()
    {
        var latestHistory = await dataAccessProvider.GetLatestEmergencyMaintenanceHistoryAsync();
        return latestHistory?.State ?? EmergencyMaintenanceState.Disabled;
    }

    public async Task SetEmergencyMaintenanceState(EmergencyMaintenanceState state, string authorEmail)
    {
        var history = new EmergencyMaintenanceHistory
        {
            State = state,
            ChangeDate = DateTime.UtcNow,
            AuthorEmail = authorEmail
        };
        await dataAccessProvider.AddEmergencyMaintenanceHistory(history);
    }
}