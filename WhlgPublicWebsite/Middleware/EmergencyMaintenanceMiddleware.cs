using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WhlgPublicWebsite.Middleware;

public class EmergencyMaintenanceMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext httpContext,
        BusinessLogic.Services.EmergencyMaintenance.EmergencyMaintenanceService emergencyMaintenanceService)
    {
        if (await emergencyMaintenanceService.SiteIsInEmergencyMaintenance())
        {
            httpContext.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            await httpContext.Response.CompleteAsync();
        }
        else
        {
            await next.Invoke(httpContext);
        }
    }
}