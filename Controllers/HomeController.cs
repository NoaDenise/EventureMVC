using EventureMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace EventureMVC.Controllers
{
    [Authorize(Roles = "User")]

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult GuestHome()
        {
            HttpContext.Session.SetString("UserRole", "Guest");
            ViewData["Message"] = "Welcome, Guest!";
            return View(); 
        }



    }
}
