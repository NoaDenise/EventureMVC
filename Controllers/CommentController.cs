using Microsoft.AspNetCore.Mvc;

namespace EventureMVC.Controllers
{
    public class CommentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
