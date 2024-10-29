using Microsoft.AspNetCore.Mvc;

namespace EventureMVC.Controllers
{
    public class RatingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
