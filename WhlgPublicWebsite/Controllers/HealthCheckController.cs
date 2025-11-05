using Microsoft.AspNetCore.Mvc;

namespace WhlgPublicWebsite.Controllers;

public class HealthCheckController : Controller
{
    [HttpGet("/health-check")]
    // Our health check uses HTTP requests, the Antiforgery token is set to secure only
    // This page contains no forms so is safe to ignore CSRF validation
    [IgnoreAntiforgeryToken]
    public IActionResult Index()
    {
        return View("Index");
    }
}