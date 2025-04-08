using Microsoft.AspNetCore.Mvc;

namespace HealthApp.Controllers
{
    public class LandingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
