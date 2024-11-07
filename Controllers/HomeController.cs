using EventureMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace EventureMVC.Controllers
{
    //[Authorize(Roles = "User")]

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

        private readonly List<string> _views = new List<string>
        {
            "Home",
            "Explore",
            "Login",
            "Register",
            "SecondActivity/AddActivity"
            // Kanske m�ste uppdatera till n�got annat, s� att den hittar alla sidor automatisktt, eller s� har vi de viktiga saker man kan s�ka p�.
        };

        public IActionResult Search(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return View("SearchResults", new List<string>());
            }

            // Filter the views based on the search query
            var results = _views.Where(v => v.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();

            return View("SearchResults", results);
        }

    }
}
