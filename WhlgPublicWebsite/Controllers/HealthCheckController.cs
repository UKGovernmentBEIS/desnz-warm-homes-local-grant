using Microsoft.AspNetCore.Mvc;

namespace WhlgPublicWebsite.Controllers
{
    public class HealthCheckController : Controller
    {
        [HttpGet("/health-check")]
        [IgnoreAntiforgeryToken]
        public IActionResult Index()
        {
            return View("Index");
        }
    }
}
