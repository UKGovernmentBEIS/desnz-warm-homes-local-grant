using System;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace HerPublicWebsite.Middleware
{
    public class BasicAuthMiddleware
    {
        private readonly RequestDelegate next;
        private readonly BasicAuthMiddlewareConfiguration configuration;

        public BasicAuthMiddleware(RequestDelegate next, IOptions<BasicAuthMiddlewareConfiguration> options)
        {
            this.next = next;
            configuration = options.Value;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext.Request.Path.StartsWithSegments(new PathString("/health-check")))
            {
                await next.Invoke(httpContext);
                return;
            }

            if (IsAuthorised(httpContext))
            {
                await next.Invoke(httpContext);
            }
            else
            {
                SendUnauthorisedResponse(httpContext);
            }
        }

        private bool IsAuthorised(HttpContext httpContext)
        {
            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(httpContext.Request.Headers["Authorization"]);
                var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, 2);
                var username = credentials[0];
                var password = credentials[1];

                return configuration.Username == username
                       && configuration.Password == password;
            }
            catch
            {
                // Default to denying access if anything goes wrong
                return false;
            }
        }

        private static void SendUnauthorisedResponse(HttpContext httpContext)
        {
            httpContext.Response.StatusCode = 401;
            AddOrUpdateHeader(httpContext, "WWW-Authenticate", $"Basic realm=\"{Constants.SERVICE_NAME}\"");
        }

        private static void AddOrUpdateHeader(HttpContext httpContext, string headerName, string headerValue)
        {
            if (httpContext.Response.Headers.ContainsKey(headerName))
            {
                httpContext.Response.Headers[headerName] = headerValue;
            }
            else
            {
                httpContext.Response.Headers.Append(headerName, headerValue);
            }
        }


    }
}
