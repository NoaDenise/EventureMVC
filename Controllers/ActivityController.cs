using Microsoft.AspNetCore.Mvc;

namespace EventureMVC.Controllers
{
    public class ActivityController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
