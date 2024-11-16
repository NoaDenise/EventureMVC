using EventureMVC.Models;
using EventureMVC.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;

namespace EventureMVC.Controllers
{
    public class MyPagesController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUri = "https://localhost:7277";

        public MyPagesController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public IActionResult Index()
        {
            // Hämta användar-ID från sessionen
            string userId = HttpContext.Session.GetString("nameid");

            // Om användaren inte är inloggad, omdirigera till login
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "User");
            }

            ViewData["Title"] = "My Pages";
            return View("Index"); // Återger vyn för My Pages
        }

        public async Task<IActionResult> MyInformation()
        {
            string userId = HttpContext.Session.GetString("nameid");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "User");
            }

            var response = await _httpClient.GetAsync($"{_baseUri}/api/User/getUserById/{userId}");
            var json = await response.Content.ReadAsStringAsync();
            var myInformation = JsonConvert.DeserializeObject<MyInformationViewModel>(json);

            return View(myInformation);
        }

        public async Task<IActionResult> SavedActivities()
        {
            ViewData["Title"] = "Saved Activities";

            // Här ska vi anropa API:et för att hämta sparade aktiviteter
            var response = await _httpClient.GetAsync($"{_baseUri}/api/User/getAllUserEvents");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var savedActivities = JsonConvert.DeserializeObject<List<SavedActivitiesUserDTO>>(json);
                return View("SavedActivities");
            }
            else
            {
                // Hantera fel här, exempelvis skicka en tom lista eller ett felmeddelande
                return View("SavedActivities", new List<SavedActivitiesUserDTO>());
            }
        }

        public async Task<IActionResult> ActivitySignUps()
        {
            string userId = "1d19e442-0828-4e8f-8b37-0752b62ab4a8";  // Byt ut detta med din autentisering

            var response = await _httpClient.GetAsync($"{_baseUri}/api/Attendance/getUsersAttendance/{userId}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var activitySignUps = JsonConvert.DeserializeObject<List<UserAttendanceDTO>>(json);

                return View("ActivitySignUps");
            }
            else
            {
                // Hantera fel vid hämtning av data från API:et
                return View("ActivitySignUps",new List<UserAttendanceDTO>());
            }
        }


        public IActionResult EditDeleteCreatedActivities()
        {
            ViewData["Title"] = "Edit/Delete Created Activities";
            return View(); 
        }
    }
}
