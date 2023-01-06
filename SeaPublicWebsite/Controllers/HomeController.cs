using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SeaPublicWebsite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet("/")]
        public IActionResult Index()
        {
            return Redirect("https://www.gov.uk/improve-energy-efficiency");
        }
    }
}