using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EventureMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;

        public AccountController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
