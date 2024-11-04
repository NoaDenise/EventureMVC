using EventureMVC.Models;
using EventureMVC.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace EventureMVC.Controllers
{
    public class SecondActivityController : Controller
    {
        private readonly HttpClient _client;
        private string baseUrl = "https://localhost:7277/"; //Lägga den här i .env kanske?
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

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddActivity(Activity activity)
        {
            if (!ModelState.IsValid)
            {
                return View(activity);
            }

            //Parsing the datetime

            var json = JsonConvert.SerializeObject(activity);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync($"{baseUrl}api/Activity/addActivity", content);
            
            if (response.IsSuccessStatusCode)
            {
                // change this to Redirect to the ActivityPage for the one you just created so u can view it
                return RedirectToAction("Success");
            }

            return View(activity);
        }
    }
}
