using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace HerPublicWebsite.Middleware;

public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate next;

    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public Task Invoke(HttpContext context)
    {
        if (!context.Response.Headers.ContainsKey("X-Content-Type-Options"))
        {
            context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
        }
        
        if (!context.Response.Headers.ContainsKey("Content-Security-Policy"))
        {
            context.Response.Headers.Add("Content-Security-Policy",
                "default-src 'self'; script-src * 'unsafe-inline' 'unsafe-eval'; connect-src  * 'unsafe-inline'"); 

        }
        
        if (!context.Response.Headers.ContainsKey("Referrer-Policy"))
        {
            context.Response.Headers.Add("Referrer-Policy", "no-referrer");

        }

        return next(context);
    } 
}