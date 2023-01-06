using Microsoft.AspNetCore.Mvc;

namespace HerPublicWebsite.Controllers
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