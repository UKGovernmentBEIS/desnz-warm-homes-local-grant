using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using WhlgPublicWebsite.BusinessLogic.Services.Password;
using WhlgPublicWebsite.Services;

namespace WhlgPublicWebsite.Middleware;

public class AuthMiddleware(RequestDelegate next)
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
        httpContext.Request.Cookies.TryGetValue(AuthService.AuthCookieName, out var auth);

        return auth != null && passwordService.HashMatchesConfiguredPassword(auth);
    }

    private static void RedirectResponseToPasswordPage(HttpContext httpContext)
    {
        var returnPath = HttpUtility.UrlEncode(httpContext.Request.Path.ToString(), Encoding.UTF8);
        httpContext.Response.Redirect($"/password?returnPath={returnPath}");
    }
}