using System.Threading.Tasks;
using HerPublicWebsite.ExternalServices.GoogleAnalytics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace HerPublicWebsite.Controllers;

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
        return Redirect("https://www.gov.uk/apply-home-upgrade-grant");
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
    
    [HttpGet("/digital-assistance")]
    public async Task<IActionResult> DigitalAssistance()
    {
        await googleAnalyticsService.SendDigitalAssistanceEventAsync(Request);
        return View("DigitalAssistance");
    }
}
