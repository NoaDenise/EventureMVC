using EventureMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Security.Claims;

namespace EventureMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _BaseUrl;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _BaseUrl = configuration["ApiSettings:BaseUrl"];
        }

        public IActionResult Index()
        {
            _logger.LogCritical($"{_BaseUrl}");
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
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
                "Activity/AddActivity"
                // Kanske m�ste uppdatera till n�got annat, s� att den hittar alla sidor automatisktt, eller s� har vi de viktiga saker man kan s�ka p�.
            };

        public IActionResult Search(string query)
        {
            //should be a control for hackers, when we have time for that...

            //removes space
            query = query?.Trim();

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
