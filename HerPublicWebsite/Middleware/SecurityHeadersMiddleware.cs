using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;

namespace HerPublicWebsite.Middleware;

public class SecurityHeadersMiddleware
{
    public const string NonceContextKey = "ContentSecurityPolicyScriptNonce";

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

        var nonce = Guid.NewGuid().ToString("N");;
        context.Items[NonceContextKey] = nonce;

        if (!context.Response.Headers.ContainsKey("Content-Security-Policy"))
        {
            // Based on https://csp.withgoogle.com/docs/strict-csp.html
            context.Response.Headers.Add("Content-Security-Policy",
                "object-src 'none'; " +
                $"script-src 'nonce-{nonce}' 'unsafe-inline' 'strict-dynamic' https:; " +
                "base-uri 'none';");
        }

        if (!context.Response.Headers.ContainsKey("Referrer-Policy"))
        {
            context.Response.Headers.Add("Referrer-Policy", "no-referrer");

        }

        return next(context);
    } 
}

public static class SecurityContextExtensions
{
    public static HtmlString GetScriptNonce(this HttpContext context)
    {
        return new HtmlString((string) context.Items[SecurityHeadersMiddleware.NonceContextKey] ?? "");
    }
}
