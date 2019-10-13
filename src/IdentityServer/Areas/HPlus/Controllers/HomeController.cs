using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Areas.HPlus.Controllers
{
    [Area("HPlus")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Page1()
        {
            return View();
        }

        public IActionResult Page2()
        {
            return View();
        }

        public IActionResult Page3()
        {
            return View();
        }
    }
}