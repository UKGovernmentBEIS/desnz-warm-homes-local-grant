using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using WhlgPublicWebsite.ExternalServices.GoogleAnalytics;

namespace WhlgPublicWebsite.Controllers;

public class StaticPagesController : Controller
{
    private readonly IWebHostEnvironment environment;
    private readonly GoogleAnalyticsService googleAnalyticsService;

    public StaticPagesController(IWebHostEnvironment environment, GoogleAnalyticsService googleAnalyticsService)
    {
        this.environment = environment;
        this.googleAnalyticsService = googleAnalyticsService;
    }

    [HttpGet("/")]
    public IActionResult Index()
    {
#if DEBUG
        return Redirect("/questionnaire/");
#endif
#pragma warning disable CS0162 // Unreachable code detected
        return Redirect(ServiceConstants.SERVICE_URL);
#pragma warning restore CS0162
    }

    [HttpGet("/accessibility-statement")]
    public IActionResult AccessibilityStatement()
    {
        return View("AccessibilityStatement");
    }

    [HttpGet("/privacy-policy")]
    public IActionResult PrivacyPolicy()
    {
        return View("PrivacyPolicy");
    }

    [HttpGet("/session-expired")]
    public IActionResult SessionExpired()
    {
        return View("SessionExpired");
    }

    [HttpGet("/digitalassistance")]
    public async Task<IActionResult> DigitalAssistance()
    {
        await googleAnalyticsService.SendDigitalAssistanceEventAsync(Request);
        return View("DigitalAssistance");
    }
}