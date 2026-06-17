using WhlgPublicWebsite.BusinessLogic.Models;

namespace WhlgPublicWebsite.BusinessLogic.Services.EmergencyMaintenance;

public class EmergencyMaintenanceService(IDataAccessProvider dataAccessProvider)
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromSeconds(30);
    private EmergencyMaintenanceState? cachedState;
    private DateTime timeLastStateFetch = DateTime.MinValue;

    public async Task<bool> SiteIsInEmergencyMaintenance()
    {
        if (IsCacheStale())
        {
            cachedState = await GetEmergencyMaintenanceState();
            timeLastStateFetch = DateTime.UtcNow;
        }

        return cachedState == EmergencyMaintenanceState.Enabled;
    }

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
        cachedState = state;
        timeLastStateFetch = DateTime.UtcNow;
    }

    private bool IsCacheStale()
    {
        return DateTime.UtcNow - timeLastStateFetch > CacheDuration;
    }
}