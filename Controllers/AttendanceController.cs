using Microsoft.AspNetCore.Mvc;

namespace EventureMVC.Controllers
{
    public class AttendanceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
