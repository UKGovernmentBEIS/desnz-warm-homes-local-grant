using Microsoft.AspNetCore.Mvc;

namespace HerPublicWebsite.Controllers;

public class StaticPagesController : Controller
{
    [HttpGet("/")]
    public IActionResult Index()
    {
        return View("Index");
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
}