using Microsoft.AspNetCore.Mvc;

namespace SeaPublicWebsite.Controllers
{
    public class HealthCheckController : Controller
    {
        [HttpGet("/health-check")]
        public IActionResult Index()
        {
            return View();
        }
    }
}