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

        [Authorize]
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

            var token = HttpContext.Request.Cookies["jwtToken"];
            if (string.IsNullOrEmpty(token))
            {
                // If the token is missing, redirect to loginpage
                return RedirectToAction("Login", "User");
            }

            // Set the authorization header for the API request
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var json = JsonConvert.SerializeObject(activity);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"{baseUrl}api/Activity/addActivity", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Success"); // Or use RedirectToAction("Details", "Activity", new { id = activity.Id });
            }

            return View(activity);
        }
    }
}
