using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using WhlgPublicWebsite.BusinessLogic.Services.Password;

namespace WhlgPublicWebsite.Middleware;

public class PasswordAuthMiddleware(RequestDelegate next)
{
    private static readonly string[] IgnoredPaths = ["/health-check", "/password", "/compiled", "/assets"];
    
    public async Task Invoke(HttpContext httpContext, PasswordService passwordService)
    {
        if (IgnoredPaths.Any(path => httpContext.Request.Path.StartsWithSegments(new PathString(path))))
        {
            await next.Invoke(httpContext);
            return;
        }

        if (IsAuthorised(httpContext, passwordService))
        {
            await next.Invoke(httpContext);
        }
        else
        {
            RedirectResponseToPasswordPage(httpContext);
        }
    }

    private static bool IsAuthorised(HttpContext httpContext, PasswordService passwordService)
    {
        httpContext.Request.Cookies.TryGetValue("Authorization", out var auth);

        return auth != null && passwordService.HashMatchesConfiguredPassword(auth);
    }

    private static void RedirectResponseToPasswordPage(HttpContext httpContext)
    {
        httpContext.Response.Redirect("/password");
    }
}

