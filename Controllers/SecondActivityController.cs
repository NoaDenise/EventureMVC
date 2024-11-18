using EventureMVC.Models;
using EventureMVC.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace EventureMVC.Controllers
{
    public class SecondActivityController : Controller
    {
        private readonly HttpClient _client;
        private readonly string baseUrl = "https://localhost:7277/"; //Lägga den här i .env kanske?
        public SecondActivityController(HttpClient client)
        {
            _client = client;
        }
        public async Task <IActionResult> Index()
        {
            return View();
        }

        public IActionResult AddActivity()
        {
            ViewData["Title"] ="Create Activity";

            var userId = HttpContext.Session.GetString("nameid");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "User");
            }

            var activity = new Activity
            {
                UserId = userId
            };

            return View(activity);
        }

        [HttpPost]
        public async Task<IActionResult> AddActivity(Activity activity)
        {
            if (!ModelState.IsValid)
            {
                return View(activity);
            }

            var userId = HttpContext.Session.GetString("nameid");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "User");
            }


            var json = JsonConvert.SerializeObject(activity);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"{baseUrl}api/Activity/addActivity", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("myCreatedActivities"); // Or use RedirectToAction("Details", "Activity", new { id = activity.Id }); Kanske ska länka där man ser listan för alla man skapat?
                //eller ändra i backend så man får ut ActivityId i svaret när man har skapat en activity.
            }

            return View(activity);
        }
    }
}

