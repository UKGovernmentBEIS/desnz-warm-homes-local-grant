using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace WhlgPublicWebsite.Services;

public class AuthService(IWebHostEnvironment webHostEnvironment)
{
    public const string AuthCookieName = "authentication";

    public bool AuthIsEnabled()
    {
        // Note IsDevelopment() returns true only on local dev, and not our deployed development environment.
        // So, this function should be true on deployed development and staging only.
        return !webHostEnvironment.IsDevelopment() && !webHostEnvironment.IsProduction();
    }
}