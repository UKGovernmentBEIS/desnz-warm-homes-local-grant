using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WhlgPublicWebsite.Services;
using WhlgPublicWebsite.Services.Cookies;

namespace WhlgPublicWebsite.Middleware;

public class CorrelationLogsMiddleware
{
    private readonly ILogger<CorrelationLogsMiddleware> logger;
    private readonly RequestDelegate next;

    public CorrelationLogsMiddleware(RequestDelegate next, ILogger<CorrelationLogsMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }

    public async Task Invoke(HttpContext httpContext, CookieService cookieService)
    {
        var questionnaireService = httpContext.RequestServices.GetRequiredService<QuestionnaireService>();
        var correlationId = questionnaireService.GetQuestionnaire().SessionId;

        var method = SanitizeContextProperty(httpContext.Request.Method);
        var path = SanitizeContextProperty(httpContext.Request.Path.ToString());

        logger.LogInformation(
            "Request started: CorrelationId={CorrelationId}, Method={safeMethod}, Path={safePath}",
            correlationId, method, path);

        var stopwatch = Stopwatch.StartNew();

        await next(httpContext);

        stopwatch.Stop();

        logger.LogInformation(
            "Request completed: CorrelationId={CorrelationId}, Method={safeMethod}, Path={safePath}, StatusCode={StatusCode}, Duration={Duration}ms",
            correlationId, method, path, httpContext.Response.StatusCode, stopwatch.ElapsedMilliseconds);
    }

    private static string SanitizeContextProperty(string property)
    {
        return property.Replace("\r", string.Empty).Replace("\n", string.Empty);
    }
}