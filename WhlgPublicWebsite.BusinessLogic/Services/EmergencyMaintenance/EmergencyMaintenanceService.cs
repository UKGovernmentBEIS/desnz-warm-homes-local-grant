using Microsoft.Extensions.DependencyInjection;
using WhlgPublicWebsite.BusinessLogic.Models;

namespace WhlgPublicWebsite.BusinessLogic.Services.EmergencyMaintenance;

public class EmergencyMaintenanceService(IServiceScopeFactory scopeFactory)
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

    private async Task<EmergencyMaintenanceState> GetEmergencyMaintenanceState()
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var dataAccessProvider = scope.ServiceProvider.GetRequiredService<IDataAccessProvider>();
        var latestHistory = await dataAccessProvider.GetLatestEmergencyMaintenanceHistoryAsync();
        return latestHistory?.State ?? EmergencyMaintenanceState.Disabled;
    }

    private bool IsCacheStale()
    {
        return DateTime.UtcNow - timeLastStateFetch > CacheDuration;
    }
}