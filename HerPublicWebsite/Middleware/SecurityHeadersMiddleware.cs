using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;

namespace HerPublicWebsite.Middleware;

public class SecurityHeadersMiddleware
{
    public const string NonceContextKey = "ContentSecurityPolicyScriptNonce";

    private const string XContentTypeOptions = "X-Content-Type-Options";
    private const string XFrameOptions = "X-Frame-Options";
    private const string ContentSecurityPolicy = "Content-Security-Policy";
    private const string ReferrerPolicy = "Referrer-Policy";

    private readonly RequestDelegate next;

    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public Task Invoke(HttpContext context)
    {
        if (!context.Response.Headers.ContainsKey(XContentTypeOptions))
        {
            context.Response.Headers.Add(XContentTypeOptions, "nosniff");
        }

        if (!context.Response.Headers.ContainsKey(XFrameOptions))
        {
            context.Response.Headers.Add(XFrameOptions, "deny");
        }

        var nonce = Guid.NewGuid().ToString("N");
        context.Items[NonceContextKey] = nonce;

        if (!context.Response.Headers.ContainsKey(ContentSecurityPolicy))
        {
            // Based on https://csp.withgoogle.com/docs/strict-csp.html
            context.Response.Headers.Add(ContentSecurityPolicy,
                "object-src 'none'; " +
                $"script-src 'nonce-{nonce}' 'unsafe-inline' 'strict-dynamic' https:; " +
                "base-uri 'none';");
        }

        if (!context.Response.Headers.ContainsKey(ReferrerPolicy))
        {
            context.Response.Headers.Add(ReferrerPolicy, "no-referrer");
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
