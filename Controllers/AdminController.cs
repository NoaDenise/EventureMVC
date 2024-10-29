using Microsoft.AspNetCore.Mvc;

namespace EventureMVC.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
